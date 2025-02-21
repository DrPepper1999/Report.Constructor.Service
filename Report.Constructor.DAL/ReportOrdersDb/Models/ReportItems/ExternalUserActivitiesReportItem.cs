namespace Report.Constructor.DAL.ReportOrdersDb.Models.ReportItems;

public sealed class ExternalUserActivitiesReportItem
{
    public Guid UserId { get; set; }
    public required string Fio { get; set; }
    public Guid CameraId { get; set; }
    public required string Title { get; set; }
    public required string Action { get; set; }
    public DateTime TimeStamp { get; set; }
    public required string ActionType { get; set; }
    public required string ExtLogin { get; set; }
    public required string Portal { get; set; }
}