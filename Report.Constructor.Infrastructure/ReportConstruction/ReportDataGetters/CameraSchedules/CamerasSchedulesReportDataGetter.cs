using MapsterMapper;
using Report.Constructor.Core.Enums;
using Report.Constructor.DAL.Interfaces.Repositories;
using Report.Constructor.Infrastructure.Interfaces.ReportConstruction;
using Report.Constructor.Infrastructure.Interfaces.ReportConstruction.Filters;
using Report.Constructor.Infrastructure.Interfaces.ReportConstruction.Getters;
using Report.Constructor.Infrastructure.Models.ReportFilters;

namespace Report.Constructor.Infrastructure.ReportConstruction.ReportDataGetters.CameraSchedules;

internal sealed class CamerasSchedulesReportDataGetter : IReportDataGetter
{
    public ReportType CompatibleReportType => ReportType.CamerasSchedules;
    public Type CompatibleReportModelType { get; } = typeof(ReportFilterModel);

    private readonly IReportDataRepository _reportDataRepository;
    private readonly IMapper _mapper;

    public CamerasSchedulesReportDataGetter(IReportDataRepository reportDataRepository, IMapper mapper)
    {
        _reportDataRepository = reportDataRepository;
        _mapper = mapper;
    }

    public async Task<IEnumerable<IReportRow>> GetReportRows(IReportFilter reportFilter)
    {
        var typedCommand = (ReportFilterModel)reportFilter;

        var items = await _reportDataRepository.GetCameraScheduleData(
            typedCommand.UserGroupsIds, typedCommand.StartDateTime, typedCommand.EndDateTime);

        var rows = _mapper.Map<IEnumerable<CameraSchedulesReportRow>>(items);

        return rows;
    }
}