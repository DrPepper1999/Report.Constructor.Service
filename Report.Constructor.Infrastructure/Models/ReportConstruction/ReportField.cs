namespace Report.Constructor.Infrastructure.Models.ReportConstruction;

internal sealed class ReportField
{
    public required string Name { get; set; }
    public required object[] Data { get; set; }
}