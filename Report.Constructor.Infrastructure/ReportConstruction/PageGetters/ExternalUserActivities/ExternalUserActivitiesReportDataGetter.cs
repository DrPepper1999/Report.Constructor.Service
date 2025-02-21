using MapsterMapper;
using Microsoft.EntityFrameworkCore;
using Report.Constructor.Application.Interfaces;
using Report.Constructor.Core.Enums;
using Report.Constructor.DAL.Interfaces.Repositories;
using Report.Constructor.DAL.MoscowVideoDb.Dto;
using Report.Constructor.Infrastructure.Extensions;
using Report.Constructor.Infrastructure.Interfaces;
using Report.Constructor.Infrastructure.Interfaces.ReportConstruction;
using Report.Constructor.Infrastructure.Interfaces.ReportConstruction.Filters;
using Report.Constructor.Infrastructure.Interfaces.ReportConstruction.Getters;
using Report.Constructor.Infrastructure.Models.ReportConstruction;
using Report.Constructor.Infrastructure.Models.ReportFilters;

namespace Report.Constructor.Infrastructure.ReportConstruction.PageGetters.ExternalUserActivities;

internal sealed class ExternalUserActivitiesReportDataGetter : IPagedDataGetter
{
    public ReportType CompatibleReportType => ReportType.ExternalUserActivities;
    public Type CompatibleReportModelType { get; } = typeof(ExternalUserActivitiesReportFilter);

    private readonly IMapper _mapper;
    private readonly ICamerasRepository _camerasRepository;
    private readonly IUsersRepository _usersRepository;
    private readonly IAuditRepository _auditRepository;
    private readonly ICameraService _cameraService;
    private readonly IApplicationIdDescriptionProvider _applicationIdDescriptionProvider;

    public ExternalUserActivitiesReportDataGetter(
        IMapper mapper,
        ICamerasRepository camerasRepository,
        IUsersRepository usersRepository,
        IAuditRepository auditRepository,
        ICameraService cameraService,
        IApplicationIdDescriptionProvider applicationIdDescriptionProvider)
    {
        _mapper = mapper;
        _camerasRepository = camerasRepository;
        _usersRepository = usersRepository;
        _auditRepository = auditRepository;
        _cameraService = cameraService;
        _applicationIdDescriptionProvider = applicationIdDescriptionProvider;
    }

    /// <summary>
    /// ExternalUserActivitiesReport
    /// </summary>
    public async Task<PagedResult> GetPage(IPageFilter reportFilter)
    {
        var typedCommand = (ExternalUserActivitiesReportFilter) reportFilter;

        var operationPlayerActionItems = await _auditRepository.GetOperationPlayerActionItems(
            typedCommand.UserId, typedCommand.AccessType?.ToString(), typedCommand.Start.ToUniversalTime(), typedCommand.End.ToUniversalTime());

        if (operationPlayerActionItems.Count == 0)
        {
            return new PagedResult(new List<IReportRow>(), 0);
        }

        var cameraIdsFilter = new HashSet<Guid>();
        
        // Выбраны камеры
        if (typedCommand is { UseExtendedSearch: false, SelectedCameraIds.Count: > 0 })
        {
            cameraIdsFilter = typedCommand.SelectedCameraIds.ToHashSet();
        }
        // Расширенный фильтр
        else if (typedCommand.UseExtendedSearch)
        {
            var extendedSearchFilter = _mapper.Map<ExtendedCameraSearchQueryFilter>(typedCommand.ExtendedSearch!);
            cameraIdsFilter = (await _cameraService.GetCameraIds(extendedSearchFilter)
                    .ToListAsync())
                .ToHashSet();
        }
        
        var externalLoginFilter = typedCommand.ExternalLogins?.Count > 0 
            ? typedCommand.ExternalLogins.ToHashSet()
            : new HashSet<string>();
        
        var cameraTitles = await _camerasRepository.GetAllCameraTitles();
        var userFullNames = await _usersRepository.GetUsersFullName();
        var descriptionsDict = await _applicationIdDescriptionProvider.GetDescriptionsAsync();

        var rows =
            (from operationPlayerAction in operationPlayerActionItems
                join cameraData in cameraTitles on operationPlayerAction.CameraId equals cameraData.Id into cameraGroup
                join userData in userFullNames on operationPlayerAction.UserId equals userData.UserId into userGroup
                from cameraData in cameraGroup.DefaultIfEmpty()
                from userData in userGroup.DefaultIfEmpty()
                where
                    (cameraIdsFilter.Count == 0 || cameraIdsFilter.Contains(operationPlayerAction.CameraId)) &&
                    (externalLoginFilter.Count == 0 || externalLoginFilter.Contains(operationPlayerAction.ExtLogin))
                select new ExternalUserActivitiesReportRow
                {
                    Fio = userData?.FullName,
                    ExtLogin = operationPlayerAction.ExtLogin,
                    Title = cameraData?.Title ?? string.Empty,
                    Action = operationPlayerAction.Action switch
                    {
                        "start" => "Начало просмотра",
                        "stop" => "Завершение просмотра",
                        "abseek" => "Поиск фрагмента (в календаре или ползунком)",
                        "scale" => "Шаг перемотки",
                        "play" => "Проигрывание видео",
                        "shift" => "Сдвиг стрелкой",
                        "rplay" => "Проигрывание видео назад",
                        "pause" => "Пауза",
                        _ => string.Empty
                    },
                    TimeStamp = operationPlayerAction.Timestamp.DateTime,
                    ActionType = operationPlayerAction.ActivityType,
                    Portal = descriptionsDict[operationPlayerAction.ApplicationId],
                    CameraId = operationPlayerAction.CameraId,
                    UserId = operationPlayerAction.UserId,
                }).ToList();

        var totalCount = rows.Count;

        var result = rows
            .OrderBy(GetPropertySelector(typedCommand.Sorting.Column), typedCommand.Sorting.Direction)
            .Paginate(typedCommand.Pagination);

        return new PagedResult(result, totalCount);
    }

    private Func<ExternalUserActivitiesReportRow, object> GetPropertySelector(string column) =>
        column switch
        {
            nameof(ExternalUserActivitiesReportRow.Action)
                or "" or null => row => row.Action,
            nameof(ExternalUserActivitiesReportRow.UserId) => row => row.UserId,
            "Name" => row => row.Fio,
            nameof(ExternalUserActivitiesReportRow.CameraId) => row => row.CameraId,
            "CameraTitle" => row => row.Title,
            nameof(ExternalUserActivitiesReportRow.ActionType) => row => row.ActionType,
            "EventTime" => row => row.TimeStamp,
            "TimeStamp" => row => row.TimeStamp,
            nameof(ExternalUserActivitiesReportRow.Portal) => row => row.Portal,
            "ActivityType" => row => row.ActionType,
            nameof(ExternalUserActivitiesReportRow.ExtLogin) => row => row.ExtLogin,
            
            _ => throw new ArgumentException($"{column} не поддерживается в качестве поля для сортировки")
        };
}