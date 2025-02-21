using Report.Constructor.Core.Enums;

namespace Report.Constructor.Core.Models;

public record ReportOrder
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public ReportOrderStatus Status { get; set; }
    public ReportType ReportType { get; set; }
    public DateTime OrderDate { get; set; }
    public ReportFileData? ReportData { get; set; }
    public required string ReportFilter { get; set; }
    public string? ErrorMessage { get; set; }
}