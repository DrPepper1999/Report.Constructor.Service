namespace Report.Constructor.DAL.ReportOrdersDb.Entities;

public class ReportOrderEntity
{
    public Guid Id { get; set; }
    public int Status { get; set; }
    public int ReportType { get; set; }
    public string? ReportPath { get; set; }
    public Guid UserId { get; set; }
    public DateTime OrderDate { get; set; }
    public required string ReportFilter { get; set; }
    public string? ErrorMessage { get; set; }
}