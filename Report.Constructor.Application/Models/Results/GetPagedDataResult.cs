namespace Report.Constructor.Application.Models.Results;

public class GetPagedDataResult
{
    public required IEnumerable<object> Items { get; set; }
    public required int TotalCount { get; set; }
}