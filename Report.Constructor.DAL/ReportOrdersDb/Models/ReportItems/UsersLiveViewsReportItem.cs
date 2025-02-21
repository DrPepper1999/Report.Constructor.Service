namespace Report.Constructor.DAL.ReportOrdersDb.Models.ReportItems;

public sealed class UsersLiveViewsReportItem
{
    public DateTime Date { get; set; }
    public required string UserFullName { get; set; }
    public required string UserGroupName { get; set; }
    public required string ParentGroupName { get; set; }
    public int? ViewsCount { get; set; }
}