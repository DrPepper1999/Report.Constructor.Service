namespace Report.Constructor.Gateway.Contracts.Responses;

internal sealed class GetPagedDataResponse
{
    public required IEnumerable<object> Items { get; set; }
    public required int TotalCount { get; set; }
}