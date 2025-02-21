using Report.Constructor.Core.Enums;

namespace Report.Constructor.Gateway.Contracts.Requests;

internal sealed record GetReportDataRequest
{
    public required ReportType ReportType { get; set; }
    public required string ReportFilter { get; set; }
};