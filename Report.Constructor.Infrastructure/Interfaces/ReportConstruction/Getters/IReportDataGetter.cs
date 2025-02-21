using Report.Constructor.Infrastructure.Interfaces.ReportConstruction.Filters;

namespace Report.Constructor.Infrastructure.Interfaces.ReportConstruction.Getters;

internal interface IReportDataGetter : IDataGetter
{
    Task<IEnumerable<IReportRow>> GetReportRows(IReportFilter reportFilter);
}