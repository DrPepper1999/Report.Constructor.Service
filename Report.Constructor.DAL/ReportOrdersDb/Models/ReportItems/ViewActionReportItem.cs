namespace Report.Constructor.DAL.ReportOrdersDb.Models.ReportItems;

public abstract class ViewActionReportItem
{
    public Guid TokenId { get; set; }
    public Guid ApplicationId { get; set; }
    public required string Action { get; set; }
    public DateTimeOffset ActionDate { get; set; }
    public string? ClientIp { get; set; }
}