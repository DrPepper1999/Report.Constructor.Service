using Report.Constructor.Core.Enums;
using Report.Constructor.DAL.ReportOrdersDb.Models.Enums;
using Report.Constructor.DAL.ReportOrdersDb.Models.ReportItems;

namespace Report.Constructor.DAL.Interfaces.Repositories;

public interface IAuditRepository
{
    Task<List<UserControlReportItem>> GetUserControls(DateTime startDateTime, DateTime endDateTime);
    Task<List<UserGroupControlReportItem>> GetUserGroupControls(DateTime startDateTime, DateTime endDateTime);
    Task<List<UserEditsReportItem>> GetUserEdits(DateTime startDateTime, DateTime endDateTime);
    Task<List<UserGroupViewsReportItem>> GetUserGroupViews(DateTime startDateTime, DateTime endDateTime);
    Task<List<UserViewsReportItem>> GetUserViews(DateTime startDateTime, DateTime endDateTime);
    Task<List<OperationPlayerActionReportItem>> GetOperationPlayerActionItems(
        Guid? userId, string? accessType, DateTime start, DateTime end);
    Task<List<OperationSharedLinkActionReportItem>> GetOperationSharedLinkActionData(DateTime dateFrom, DateTime dateTo);
    Task<List<OperationOrderArchiveReportItem>> GetOperationOrderArchiveData(DateTime dateFrom, DateTime dateTo);
    Task<List<OperationScreenshotJobCrudReportItem>> GetOperationScreenshotJobCrudData(
        DateTime dateFrom, DateTime dateTo, CrudOperationType operationType);
    Task<List<LiveViewReportItem>> GetLiveViewData(DateTime dateFrom, DateTime dateTo, IEnumerable<Guid> tokenIds);
    Task<List<ArchiveViewReportItem>> GetArchiveViewData(
        DateTime dateFrom, DateTime dateTo, IEnumerable<Guid> tokenIds, IEnumerable<ArchiveViewActionType> actionTypes);
    Task<List<ArchiveViewReportItem>> GetArchiveView(Guid tokenId, DateTimeOffset start, DateTimeOffset end);
}