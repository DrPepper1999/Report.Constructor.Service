namespace Report.Constructor.DAL.ReportOrdersDb.Models.ReportItems;

public sealed record OperationSharedLinkActionReportItem(Guid Id, Guid UserId, Guid CameraId, int LinkType, int ActionType);
