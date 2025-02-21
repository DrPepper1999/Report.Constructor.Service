using Report.Constructor.Core.Enums;

namespace Report.Constructor.Application.Models.Queries;

public sealed record GetReportDataQuery
{
    public required ReportType ReportType { get; set; }
    public required string ReportFilter { get; set; }
}