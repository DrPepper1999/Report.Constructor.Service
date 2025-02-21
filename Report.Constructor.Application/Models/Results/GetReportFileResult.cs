using Report.Constructor.Core.Enums;

namespace Report.Constructor.Application.Models.Results;

public sealed class GetReportFileResult
{
    public required ReportOrderStatus Status { get; set; }
    public string? DownloadName { get; set; }
    public byte[]? ReportData { get; set; }
    public string? ErrorMessage { get; set; }
}