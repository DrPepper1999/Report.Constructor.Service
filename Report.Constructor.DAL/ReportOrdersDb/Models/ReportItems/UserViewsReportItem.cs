namespace Report.Constructor.DAL.ReportOrdersDb.Models.ReportItems;

public sealed record UserViewsReportItem(Guid UserId, DateTime Date, int ViewCount);