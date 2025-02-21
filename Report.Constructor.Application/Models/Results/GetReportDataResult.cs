namespace Report.Constructor.Application.Models.Results;

public sealed record GetReportDataResult
{
    public required IEnumerable<object> ReportRows { get; set; }
}