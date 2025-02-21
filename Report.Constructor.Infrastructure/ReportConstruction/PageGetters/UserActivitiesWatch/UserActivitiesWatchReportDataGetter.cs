using MapsterMapper;
using Microsoft.EntityFrameworkCore;
using Report.Constructor.Application.Interfaces;
using Report.Constructor.Core.Enums;
using Report.Constructor.DAL.Interfaces.Repositories;
using Report.Constructor.DAL.MoscowVideoDb.Dto;
using Report.Constructor.DAL.MoscowVideoDb.Enums;
using Report.Constructor.DAL.ReportOrdersDb.Models.ReportItems;
using Report.Constructor.Infrastructure.Extensions;
using Report.Constructor.Infrastructure.Interfaces;
using Report.Constructor.Infrastructure.Interfaces.ReportConstruction.Filters;
using Report.Constructor.Infrastructure.Interfaces.ReportConstruction.Getters;
using Report.Constructor.Infrastructure.Models.ReportConstruction;
using Report.Constructor.Infrastructure.Models.ReportFilters;

namespace Report.Constructor.Infrastructure.ReportConstruction.PageGetters.UserActivitiesWatch;

internal sealed class UserActivitiesWatchReportDataGetter : IPagedDataGetter
{
    public ReportType CompatibleReportType => ReportType.UserActivitiesWatch;
    public Type CompatibleReportModelType { get; } = typeof(UserActivitiesWatchReportFilter);

    private readonly IMapper _mapper;
    private readonly ITokenRepository _tokenRepository;
    private readonly IUsersRepository _usersRepository;
    private readonly ICameraService _cameraService;
    private readonly IAuditRepository _auditRepository;
    private readonly IApplicationIdDescriptionProvider _applicationIdDescriptionProvider;

    public UserActivitiesWatchReportDataGetter(
        IMapper mapper,
        ITokenRepository tokenRepository,
        IUsersRepository usersRepository,
        ICameraService cameraService,
        IAuditRepository auditRepository,
        IApplicationIdDescriptionProvider applicationIdDescriptionProvider)
    {
        _mapper = mapper;
        _tokenRepository = tokenRepository;
        _usersRepository = usersRepository;
        _cameraService = cameraService;
        _auditRepository = auditRepository;
        _applicationIdDescriptionProvider = applicationIdDescriptionProvider;
    }
    
    public async Task<PagedResult> GetPage(IPageFilter reportFilter)
    {
        var typedFilter = (UserActivitiesWatchReportFilter)reportFilter;
        ValidateFilter(typedFilter);

        var tokensQuery = _tokenRepository.GetTokensForUserActivityWatchAsync(
            typedFilter.DateStart.ToLocalTime(),
            typedFilter.DateEnd.ToLocalTime(),
            typedFilter.UserId,
            typedFilter.ActivityType);
        List<TokenForUserActivityWatchOutDto> tokens;
        
        // Без фильтрации камер
        if ((typedFilter.SelectedCameraIds == null || typedFilter.SelectedCameraIds.Count == 0)
            && !typedFilter.UseExtendedSearch)
        {
            tokens = await tokensQuery.ToListAsync();
        }
        // Выбраны камеры
        else if (typedFilter.SelectedCameraIds is { Count: > 0 })
        {
            tokens = await tokensQuery
                .Where(t => typedFilter.SelectedCameraIds.Contains(t.CameraId))
                .ToListAsync();
        }
        // Расширенный фильтр
        else
        {
            var extendedSearchFilter = _mapper.Map<ExtendedCameraSearchQueryFilter>(typedFilter.ExtendedSearch!);
            var cameraIdsQuery = _cameraService.GetCameraIds(extendedSearchFilter);
            
            tokens = await tokensQuery
                .Where(t => cameraIdsQuery.Contains(t.CameraId))
                .ToListAsync();
        }

        var archiveViewTokenIds = tokens
            .Where(t => t is { ActivityType: VideoAccessType.Archive })
            .Select(t => t.TokenId);
        var liveViewTokenIds = tokens
            .Where(t => t is { ActivityType: VideoAccessType.Live })
            .Select(t => t.TokenId);

        var archiveViewActions = await _auditRepository.GetArchiveViewData(
            typedFilter.DateStart, typedFilter.DateEnd, archiveViewTokenIds,
            new[] { ArchiveViewActionType.start, ArchiveViewActionType.stop });
        var liveViewActions = await _auditRepository.GetLiveViewData(
            typedFilter.DateStart, typedFilter.DateEnd, liveViewTokenIds);

        var descriptionsDict = await _applicationIdDescriptionProvider.GetDescriptionsAsync();
        var archiveViews = ViewActionsToReportRows(archiveViewActions, descriptionsDict);
        var liveViews = ViewActionsToReportRows(liveViewActions, descriptionsDict);
        var views = archiveViews.Concat(liveViews).ToList();

        var totalCount = views.Count;

        var tokenIdToToken = tokens.ToDictionary(x => x.TokenId);
        foreach (var view in views)
        {
            var token = tokenIdToToken[view.TokenId];
            
            view.UserId = token.UserId;
            view.UserFullName = token.UserFullName;
            view.UserGroupName = token.UserGroupName;
            view.CameraId = token.CameraId;
            view.CameraTitle = token.CameraTitle ?? string.Empty;
            view.ActivityType = token.ActivityType;
            view.ActivityTypeName = token.ActivityType.ToString()?.ToLower() ?? string.Empty;
        }
        
        views = views
            .OrderBy(GetSortPropertySelector(typedFilter.Sorting.Column), typedFilter.Sorting.Direction)
            .Paginate(typedFilter.Pagination)
            .ToList();

        await SetLoginsToTokensAsync(views);
        
        return new PagedResult(views.ToList(), totalCount);
    }

    private void ValidateFilter(UserActivitiesWatchReportFilter reportFilter)
    {
        if (reportFilter.DateStart > reportFilter.DateEnd)
            throw new ArgumentException(
                $"{nameof(reportFilter.DateStart)} не может быть больше {nameof(reportFilter.DateEnd)}");
    }

    /// <summary>Сжатие действий при просмотре в вид сессий.</summary>
    /// <param name="actions">Атомарные действия при просмотре.</param>
    /// <param name="descriptionsDict">Описания applicationIds.</param>
    /// <returns>Сессии просмотра. Содержат время начала/окончания просмотра (без промежуточных действий).</returns>
    private IEnumerable<UserActivitiesWatchReportRow> ViewActionsToReportRows(
        IEnumerable<ViewActionReportItem> actions,
        Dictionary<Guid,string> descriptionsDict)
    {
        var rows = actions.GroupBy(
            g => new { g.TokenId, g.ClientIp },
            (_, matches) =>
            {
                var (appealStart, appealEnd) = GetAppealInterval(matches);
                var anyMatch = matches.First();
                
                return new UserActivitiesWatchReportRow
                {
                    TokenId = anyMatch.TokenId,
                    AppealStart = appealStart,
                    AppealEnd = appealEnd,
                    RequestSourcePortal = descriptionsDict[anyMatch.ApplicationId],
                    AppealSourceIp = anyMatch.ClientIp
                };
            });
        return rows;
    }

    /// <summary>
    /// Получение временного интервала, в течение которого происходил просмотр.
    /// Учитываем, что возможна пауза при просмотре, поэтому действия "start" и "stop" неоднократны.
    /// </summary>
    /// <param name="actions">Список действий под токеном. Подразумевается, что у всех записей TokenId одинаков.</param>
    /// <returns>Временной интервал начала и конца просмотра.</returns>
    private (DateTimeOffset? Start, DateTimeOffset? Stop) GetAppealInterval(IEnumerable<ViewActionReportItem> actions)
    {
        var firstStart = actions.FirstOrDefault(x => x.Action == "start")
            ?.ActionDate;
        var lastStop = actions.LastOrDefault(x => x.Action == "stop")
            ?.ActionDate;

        var appealStart = (firstStart.HasValue && !lastStop.HasValue) || firstStart < lastStop
            ? firstStart
            : null;

        var appealEnd = (lastStop.HasValue && !firstStart.HasValue) || firstStart < lastStop
            ? lastStop
            : null;

        return (appealStart, appealEnd);
    }
    
    private Func<UserActivitiesWatchReportRow, object> GetSortPropertySelector(string column) =>
        column.Trim() switch
        {
            nameof(UserActivitiesWatchReportRow.AppealStart) => row => row.AppealStart,
            nameof(UserActivitiesWatchReportRow.AppealEnd) => row => row.AppealEnd,
            nameof(UserActivitiesWatchReportRow.ActivityType) => row => row.ActivityType,
            "" or null => row => row.AppealStart,

            _ => throw new ArgumentException($"{column} не поддерживается в качестве поля для сортировки")
        };

    private async Task SetLoginsToTokensAsync(ICollection<UserActivitiesWatchReportRow> rows)
    {
        var userIds = rows.Select(x => x.UserId).Distinct().ToList();
        var userIdToLogin = await _usersRepository.GetUsersLoginAsync(userIds);
        
        foreach (var item in rows)
        {
            item.UserLogin = userIdToLogin.GetValueOrDefault(item.UserId, string.Empty);
        }
    }
}