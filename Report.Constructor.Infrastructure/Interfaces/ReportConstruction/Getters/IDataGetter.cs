using Report.Constructor.Core.Enums;

namespace Report.Constructor.Infrastructure.Interfaces.ReportConstruction.Getters;

internal interface IDataGetter
{
    ReportType CompatibleReportType { get; }
    Type CompatibleReportModelType { get; }
}