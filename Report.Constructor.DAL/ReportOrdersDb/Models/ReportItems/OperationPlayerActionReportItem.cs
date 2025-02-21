namespace Report.Constructor.DAL.ReportOrdersDb.Models.ReportItems;

public record OperationPlayerActionReportItem(
    Guid UserId,
    Guid CameraId,
    string Action,
    Guid ApplicationId,
    DateTimeOffset Timestamp,
    string? ActivityType,
    string? ExtLogin);
