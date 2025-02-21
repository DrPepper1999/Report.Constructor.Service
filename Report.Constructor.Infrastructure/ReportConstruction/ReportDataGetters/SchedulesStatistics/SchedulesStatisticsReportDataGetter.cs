using MapsterMapper;
using Report.Constructor.Core.Enums;
using Report.Constructor.DAL.Interfaces.Repositories;
using Report.Constructor.Infrastructure.Interfaces.ReportConstruction;
using Report.Constructor.Infrastructure.Interfaces.ReportConstruction.Filters;
using Report.Constructor.Infrastructure.Interfaces.ReportConstruction.Getters;
using Report.Constructor.Infrastructure.Models.ReportFilters;

namespace Report.Constructor.Infrastructure.ReportConstruction.ReportDataGetters.SchedulesStatistics;

internal sealed class SchedulesStatisticsReportDataGetter : IReportDataGetter
{
    public ReportType CompatibleReportType => ReportType.SchedulesStatistics;
    public Type CompatibleReportModelType { get; } = typeof(ReportFilterModel);

    private readonly IReportDataRepository _reportDataRepository;
    private readonly IMapper _mapper;

    public SchedulesStatisticsReportDataGetter(IReportDataRepository reportDataRepository, IMapper mapper)
    {
        _reportDataRepository = reportDataRepository;
        _mapper = mapper;
    }

    public async Task<IEnumerable<IReportRow>> GetReportRows(IReportFilter reportFilter)
    {
        var typedCommand = (ReportFilterModel)reportFilter;

        var items = await _reportDataRepository.GetSchedulesStatistics(
            typedCommand.StartDateTime,
            typedCommand.EndDateTime,
            typedCommand.UserGroupsIds);

        var rows = _mapper.Map<IEnumerable<SchedulesStatisticsReportRow>>(items);

        return rows;
    }
}