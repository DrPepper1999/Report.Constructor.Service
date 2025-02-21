using Report.Constructor.Infrastructure.Interfaces.ReportConstruction.Filters;

namespace Report.Constructor.Infrastructure.Interfaces.ReportConstruction;

internal interface IReportFilterNormalizer
{
    IFilter NormalizeFilter(IFilter filter);
}