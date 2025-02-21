using Report.Constructor.Core.Enums;

namespace Report.Constructor.Gateway.Contracts.Requests;

internal sealed record CreateReportRequest
{
    public required ReportType ReportType { get; set; }
    public required Guid UserId { get; set; }
    public required string ReportFilter { get; set; }
};