using MapsterMapper;
using Report.Constructor.Core.Enums;
using Report.Constructor.DAL.Interfaces.Repositories;
using Report.Constructor.Infrastructure.Interfaces.ReportConstruction;
using Report.Constructor.Infrastructure.Interfaces.ReportConstruction.Filters;
using Report.Constructor.Infrastructure.Interfaces.ReportConstruction.Getters;
using Report.Constructor.Infrastructure.Models.ReportFilters;

namespace Report.Constructor.Infrastructure.ReportConstruction.ReportDataGetters.UserGroupActions;

internal sealed class UserGroupActionsReportDataGetter : IReportDataGetter
{
    public ReportType CompatibleReportType => ReportType.UserGroupActions;
    public Type CompatibleReportModelType { get; } = typeof(ReportFilterModel);

    private readonly IReportDataRepository _reportDataRepository;
    private readonly IMapper _mapper;

    public UserGroupActionsReportDataGetter(IReportDataRepository reportDataRepository, IMapper mapper)
    {
        _reportDataRepository = reportDataRepository;
        _mapper = mapper;
    }

    public async Task<IEnumerable<IReportRow>> GetReportRows(IReportFilter reportFilter)
    {
        var typedCommand = (ReportFilterModel)reportFilter;

        var items = await _reportDataRepository.GetUserGroupActions(
            typedCommand.StartDateTime,
            typedCommand.EndDateTime,
            typedCommand.UserGroupsIds);

        var rows = _mapper.Map<IEnumerable<UserGroupActionsReportRow>>(items);

        return rows;
    }
}