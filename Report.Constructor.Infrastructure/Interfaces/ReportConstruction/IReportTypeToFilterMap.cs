using Report.Constructor.Core.Enums;
using Report.Constructor.Infrastructure.Interfaces.ReportConstruction.Filters;

namespace Report.Constructor.Infrastructure.Interfaces.ReportConstruction;

internal interface IReportTypeToFilterMap
{
    IFilter BuildFilterForReportType(ReportType reportType, string jsonBody);
}