namespace Report.Constructor.Core.Models;

public record ReportFilter
{
    public DateTime StartDateTime { get; set; }
    public DateTime EndDateTime { get; set; }
}