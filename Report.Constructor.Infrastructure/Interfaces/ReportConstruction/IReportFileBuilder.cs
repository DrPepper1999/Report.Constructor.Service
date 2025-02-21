using Report.Constructor.Infrastructure.Models.ReportConstruction;

namespace Report.Constructor.Infrastructure.Interfaces.ReportConstruction;

internal interface IReportFileBuilder
{
    Task<string> BuildReportFile(ReportData reportData);
}