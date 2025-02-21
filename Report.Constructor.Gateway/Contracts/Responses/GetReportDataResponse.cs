namespace Report.Constructor.Gateway.Contracts.Responses;

internal sealed class GetReportDataResponse
{
    public required IEnumerable<object> ReportRows { get; set; }
}