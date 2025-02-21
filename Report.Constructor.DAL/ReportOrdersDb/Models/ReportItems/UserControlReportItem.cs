namespace Report.Constructor.DAL.ReportOrdersDb.Models.ReportItems;

public sealed record UserControlReportItem(Guid UserId, DateTime Date, Guid CameraId, int Count);