namespace Report.Constructor.DAL.ReportOrdersDb.Models.ReportItems;

public sealed record UserGroupControlReportItem(Guid UserId, DateTime Date, int Count);