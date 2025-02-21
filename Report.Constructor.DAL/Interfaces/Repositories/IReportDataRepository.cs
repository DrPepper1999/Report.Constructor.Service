using Report.Constructor.DAL.ReportOrdersDb.Models.ReportItems;
using Report.Constructor.DAL.ReportsDb.Entities;

namespace Report.Constructor.DAL.Interfaces.Repositories;

public interface IReportDataRepository
{
    Task<List<UserAction>> GetUserActions(DateTime startDateTime, DateTime endDateTime);
    Task<IEnumerable<UsersLiveViewsReportItem>> GetUsersLiveViews(DateTime dateFrom, DateTime dateTo, ICollection<Guid> userGroupsIds);
    Task<IEnumerable<EchdUserActionsReportItem>> GetEchdUserActions(DateTime dateFrom, DateTime dateTo, ICollection<Guid> userGroupsIds);

    Task<IEnumerable<UserGroupActionsReportItem>> GetUserGroupActions(DateTime dateFrom, DateTime dateTo,
        ICollection<Guid> userGroupsIds);

    Task<IEnumerable<CreatedScreenshotsReportItem>> GetCreatedScreenshotsData(
        IEnumerable<OperationScreenshotJobCrudReportItem> operationScreenshotJobCrudReportItems,
        DateTime dateFrom, 
        DateTime dateTo,
        ICollection<Guid> userGroupsIds);
    
    Task<IEnumerable<ArchiveDownloadsReportItem>> GetArchiveDownloads(DateTime dateFrom, DateTime dateTo,
        ICollection<Guid> userGroupsIds);

    Task<IEnumerable<CameraSchedulesReportItem>> GetCameraScheduleData(ICollection<Guid> userGroupsIds,
        DateTime dateFrom, DateTime dateTo);

    Task<IEnumerable<SchedulesStatisticsReportItem>> GetSchedulesStatistics(DateTime dateFrom, DateTime dateTo,
        ICollection<Guid> userGroupsIds);

    Task<IEnumerable<LinksUsageReportItem>> GetLinksUsage(
        IEnumerable<OperationSharedLinkActionReportItem> operationSharedLinkActionReportItems,
        ICollection<Guid> userGroupsIds);
    
    Task<int> GetExternalUserActivitiesTotalCountAsync(
        List<Guid> cameraIds,
        Guid? userId,
        string? accessType,
        DateTime start,
        DateTime end,
        List<string> externalLogins);
}