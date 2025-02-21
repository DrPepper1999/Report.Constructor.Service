using MapsterMapper;
using Report.Constructor.Application.Interfaces;
using Report.Constructor.Core.Enums;
using Report.Constructor.Core.Extensions;
using Report.Constructor.DAL.Interfaces.Repositories;
using Report.Constructor.DAL.MoscowVideoDb.Entities;
using Report.Constructor.DAL.MoscowVideoDb.Enums;
using Report.Constructor.Infrastructure.Interfaces.ReportConstruction;
using Report.Constructor.Infrastructure.Interfaces.ReportConstruction.Filters;
using Report.Constructor.Infrastructure.Interfaces.ReportConstruction.Getters;
using Report.Constructor.Infrastructure.Models.ReportFilters;

namespace Report.Constructor.Infrastructure.ReportConstruction.ReportDataGetters.CameraView;

internal sealed class CameraViewStatisticsReportDataGetter : IReportDataGetter
{
    public ReportType CompatibleReportType => ReportType.CameraViewReport;
    public Type CompatibleReportModelType { get; } = typeof(ReportFilterModel);

    private readonly IMapper _mapper;
    private readonly ITokenRepository _tokenRepository;
    private readonly IAuditRepository _auditRepository;
    private readonly IUsersRepository _usersRepository;
    private readonly IApplicationIdDescriptionProvider _applicationIdDescriptionProvider;
    private readonly IUserGroupRepository _userGroupRepository;

    public CameraViewStatisticsReportDataGetter(
        IMapper mapper,
        ITokenRepository tokenRepository,
        IAuditRepository auditRepository,
        IUsersRepository usersRepository,
        IUserGroupRepository userGroupRepository, 
        IApplicationIdDescriptionProvider applicationIdDescriptionProvider)
    {
        _mapper = mapper;
        _tokenRepository = tokenRepository;
        _auditRepository = auditRepository;
        _usersRepository = usersRepository;
        _userGroupRepository = userGroupRepository;
        _applicationIdDescriptionProvider = applicationIdDescriptionProvider;
    }

    /// <summary>
    /// CameraViewsStatisticsReport
    /// </summary>
    public async Task<IEnumerable<IReportRow>> GetReportRows(IReportFilter reportFilter)
    {
        var typedCommand = (ReportFilterModel)reportFilter;

        if (typedCommand.CameraId is null)
        {
            throw new ArgumentException("Camera id cannot be null for this report type");
        }

        var tokens = await _tokenRepository.GetTokensAsync(
            typedCommand.StartDateTime, 
            typedCommand.EndDateTime, 
            typedCommand.CameraId.Value,
            new[]
            { 
                VideoAccessType.Archive,
                VideoAccessType.Live
            });

        var tokenGroups = tokens
            .Where(t => t.ActivityType.HasValue)
            .GroupBy(t => t.ActivityType!.Value)
            .ToDictionary(g => g.Key, g => g.Select(t => t));

        var archiveViewTokens = tokenGroups.GetValueOrDefault(VideoAccessType.Archive, Enumerable.Empty<Token>()).ToArray();
        var liveViewTokens = tokenGroups.GetValueOrDefault(VideoAccessType.Live, Enumerable.Empty<Token>()).ToArray();
        
        var tempArchiveViews = await _auditRepository.GetArchiveViewData(
            typedCommand.StartDateTime, 
            typedCommand.EndDateTime,
            archiveViewTokens.Select(t => t.Id),
            new[]
            {
                ArchiveViewActionType.start,
                ArchiveViewActionType.stop,
            });
            
        var tempLiveViews = await _auditRepository.GetLiveViewData(
            typedCommand.StartDateTime,
            typedCommand.EndDateTime,
            liveViewTokens.Select(t => t.Id));

        var allUsers = await _usersRepository.GetAllAsync();
        var allUserGroups = await _userGroupRepository.GetAllAsync();
        var allUsersLogins = await _usersRepository.GetUsersLoginsAsync(
            allUsers.Select(x => x.Id).ToList());
        
        var userGroupIdFilter = typedCommand.UserGroupsIds.Length > 0 
            ? typedCommand.UserGroupsIds.ToHashSet()
            : new HashSet<Guid>();

        var externalServicesDict = await _applicationIdDescriptionProvider.GetDescriptionsAsync();

        var archiveViewsResult =
            from archiveView in tempArchiveViews
            join archiveViewToken in archiveViewTokens on archiveView.TokenId equals archiveViewToken.Id
            join user in allUsers on archiveViewToken.UserId equals user.Id
            join userGroup in allUserGroups on user.UserGroupId equals userGroup.Id
            where userGroupIdFilter.Count == 0 || userGroupIdFilter.Contains(userGroup.Id)
            select new CameraViewStatisticsReportRow
            {
                Date = archiveView.ActionDate.DateTime,
                Action = archiveView.Action.FirstCharToUpper(),
                UserName = $"{user.LastName} {user.FirstName} {user.MiddleName}",
                UserGroupName = userGroup.FullName ?? string.Empty,
                ActiveDirectoryLogin = allUsersLogins.GetValueOrDefault(user.Id)?.AdLogin,
                SudirLogin = allUsersLogins.GetValueOrDefault(user.Id)?.SudirLogin,
                Type = "Архив",
                Ip = archiveView.ClientIp,
                PortalType = externalServicesDict[archiveView.ApplicationId]
            };

        var liveViewsResult =
            from liveViews in tempLiveViews
            join liveViewToken in liveViewTokens on liveViews.TokenId equals liveViewToken.Id
            join user in allUsers on liveViewToken.UserId equals user.Id
            join userGroup in allUserGroups on user.UserGroupId equals userGroup.Id
            where userGroupIdFilter.Count == 0 || userGroupIdFilter.Contains(userGroup.Id)
            select new CameraViewStatisticsReportRow
            {
                Date = liveViews.ActionDate.DateTime,
                Action = liveViews.Action.FirstCharToUpper(),
                UserName = $"{user.LastName} {user.FirstName} {user.MiddleName}",
                UserGroupName = userGroup.FullName ?? string.Empty,
                ActiveDirectoryLogin = allUsersLogins.GetValueOrDefault(user.Id)?.AdLogin,
                SudirLogin = allUsersLogins.GetValueOrDefault(user.Id)?.SudirLogin,
                Type = "Лайв",
                Ip = liveViews.ClientIp,
                PortalType = externalServicesDict[liveViews.ApplicationId]
            };

        var items = archiveViewsResult
            .Concat(liveViewsResult)
            .OrderBy(x => x.Date)
            .ThenByDescending(x => x.Action);

        var rows = _mapper.Map<IEnumerable<CameraViewStatisticsReportRow>>(items);

        return rows;
    }
}