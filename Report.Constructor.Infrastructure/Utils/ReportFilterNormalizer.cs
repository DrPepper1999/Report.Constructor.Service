using Microsoft.Extensions.Options;
using Report.Constructor.Core.Options;
using Report.Constructor.Infrastructure.Interfaces.ReportConstruction;
using Report.Constructor.Infrastructure.Interfaces.ReportConstruction.Filters;
using Report.Constructor.Infrastructure.Models.ReportFilters;

namespace Report.Constructor.Infrastructure.Utils;

internal sealed class ReportFilterNormalizer : IReportFilterNormalizer
{
    private readonly TimeSpan _maxReportDataAge;
    
    public ReportFilterNormalizer(IOptions<ReportsOptions> reportsOptions)
    {
        _maxReportDataAge = reportsOptions.Value.MaxReportDataAge;
    }
    
    public IFilter NormalizeFilter(IFilter filter)
    {
        var now = DateTime.Now;

        switch (filter)
        {
            case ExternalUserActivitiesReportFilter externalUserActivitiesReportFilter:
                externalUserActivitiesReportFilter.Start = now - externalUserActivitiesReportFilter.Start > _maxReportDataAge
                    ? now.Add(-_maxReportDataAge).Date
                    : externalUserActivitiesReportFilter.Start;
                break;
            case ReportFilterModel reportFilterModel:
                reportFilterModel.StartDateTime = now - reportFilterModel.StartDateTime > _maxReportDataAge
                    ? now.Add(-_maxReportDataAge).Date
                    : reportFilterModel.StartDateTime;
                break;
        }
        
        return filter;
    }
}