namespace Report.Constructor.DAL.ReportOrdersDb.Models.ReportItems;

public sealed record OperationOrderArchiveReportItem(
    int ArchiveNumber,
    DateTimeOffset BeginTime,
    DateTimeOffset EndTime,
    Guid UserId,
    Guid CameraId,
    DateTimeOffset Timestamp,
    string Reason,
    Guid ArchiveTaskId);
