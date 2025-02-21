using Report.Constructor.Core.Enums;
using Report.Constructor.DAL.Interfaces.Repositories;
using Report.Constructor.Infrastructure.Interfaces.ReportConstruction;
using Report.Constructor.Infrastructure.Interfaces.ReportConstruction.Filters;
using Report.Constructor.Infrastructure.Interfaces.ReportConstruction.Getters;
using Report.Constructor.Infrastructure.Models.ReportFilters;

namespace Report.Constructor.Infrastructure.ReportConstruction.ReportDataGetters.ArchiveViews;

internal sealed class ArchiveViewsReportDataGetter : IReportDataGetter
{
    public ReportType CompatibleReportType => ReportType.ArchiveActivities;
    public Type CompatibleReportModelType { get; } = typeof(ArchiveViewsReportFilter);

    private readonly IAuditRepository _auditRepository;

    public ArchiveViewsReportDataGetter(IAuditRepository auditRepository)
    {
        _auditRepository = auditRepository;
    }
    
    public async Task<IEnumerable<IReportRow>> GetReportRows(IReportFilter reportFilter)
    {
        var typedFilter = (ArchiveViewsReportFilter)reportFilter;
        ValidateFilter(typedFilter);
        
        var archiveViews = await _auditRepository.GetArchiveView(
            typedFilter.TokenId, typedFilter.Start, typedFilter.End);

        var rows = archiveViews.Select(x => new ArchiveViewsReportRow
        {
            Action = x.Action,
            ActionStart = x.ActionDate,
            TimeInArchive = x.TimeInArchive
        });
        
        return rows;
    }

    private void ValidateFilter(ArchiveViewsReportFilter reportFilter)
    {
        if (reportFilter.Start > reportFilter.End)
            throw new ArgumentException(
                $"{nameof(reportFilter.Start)} не может быть больше {nameof(reportFilter.End)}");
    }
}