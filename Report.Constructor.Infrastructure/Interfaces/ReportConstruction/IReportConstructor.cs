using Report.Constructor.Core.Enums;
using Report.Constructor.Core.Models;
using Report.Constructor.Infrastructure.Models.ReportConstruction;

namespace Report.Constructor.Infrastructure.Interfaces.ReportConstruction;

internal interface IReportConstructor
{
    Task<ReportFileData> ConstructReport(ReportType reportType, string reportFilter);
    Task<ReportResult> GetReportResultAsync(ReportType reportType, string reportFilter);
    Task<PagedResult> GetPagedDataAsync(ReportType reportType, string reportFilter);
}