namespace Report.Constructor.Gateway.Models;

internal sealed record ErrorDetails
{
    public required int StatusCode { get; init; }
    public required string Message { get; init; }
}