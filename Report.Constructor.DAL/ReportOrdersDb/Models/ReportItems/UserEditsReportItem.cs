namespace Report.Constructor.DAL.ReportOrdersDb.Models.ReportItems;

public sealed record UserEditsReportItem(DateTime Date, Guid UserId, int Count);