using Report.Constructor.Infrastructure.Interfaces.ReportConstruction.Filters;
using Report.Constructor.Infrastructure.Models.ReportConstruction;

namespace Report.Constructor.Infrastructure.Interfaces.ReportConstruction.Getters;

internal interface IPagedDataGetter : IDataGetter
{
    Task<PagedResult> GetPage(IPageFilter reportFilter);
}