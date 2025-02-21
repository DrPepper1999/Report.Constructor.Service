namespace Report.Constructor.DAL.ReportOrdersDb.Models.ReportItems;

public sealed class CameraViewsStatisticsReportItem
{
    public DateTime Date { get; set; }
    public required string Action { get; set; }
    public required string UserFullName { get; set; }
    public required string UserGroupName { get; set; }
    public required string ActiveDirectoryLogin { get; set; }
    public required string SudirLogin { get; set; }
    public required string Type { get; set; }
    public required string Ip { get; set; }
    public required string PortalType { get; set; }
}