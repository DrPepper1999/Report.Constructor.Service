namespace Report.Constructor.DAL.ReportOrdersDb.Models.ReportItems;

public sealed record UserGroupViewsReportItem(Guid UserId, DateTime Date, int ViewCount);