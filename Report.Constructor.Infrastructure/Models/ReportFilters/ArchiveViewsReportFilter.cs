using Report.Constructor.Infrastructure.Interfaces.ReportConstruction.Filters;

namespace Report.Constructor.Infrastructure.Models.ReportFilters;

public sealed record ArchiveViewsReportFilter : IReportFilter
{
    public Guid TokenId { get; init; }
    public DateTimeOffset Start { get; set; }
    public DateTimeOffset End { get; set; }
}