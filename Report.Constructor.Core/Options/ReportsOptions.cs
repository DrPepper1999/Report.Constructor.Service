using System.ComponentModel.DataAnnotations;

namespace Report.Constructor.Core.Options;

public class ReportsOptions
{
    public string ReportsPath { get; set; } = default!;
    
    public TimeSpan MaxReportDataAge { get; set; }
    
    [Range(1, 100_000)]
    public int AuditReportPageSize { get; set; }
    
    [Range(1, int.MaxValue)]
    public int MaxPage { get; set; }
    
    public TimeSpan AuditDataRetrievingTimeout { get; set; }
}