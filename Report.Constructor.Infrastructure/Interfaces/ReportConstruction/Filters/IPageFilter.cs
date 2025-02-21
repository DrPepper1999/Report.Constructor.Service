using Report.Constructor.Infrastructure.Models.ReportFilters;

namespace Report.Constructor.Infrastructure.Interfaces.ReportConstruction.Filters;

internal interface IPageFilter : IFilter
{
    public Sorting Sorting { get; set; }
    public Pagination? Pagination { get; set; }
}