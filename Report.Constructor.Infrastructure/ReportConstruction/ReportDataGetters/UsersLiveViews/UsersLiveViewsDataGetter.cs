using MapsterMapper;
using Report.Constructor.Core.Enums;
using Report.Constructor.DAL.Interfaces.Repositories;
using Report.Constructor.Infrastructure.Interfaces.ReportConstruction;
using Report.Constructor.Infrastructure.Interfaces.ReportConstruction.Filters;
using Report.Constructor.Infrastructure.Interfaces.ReportConstruction.Getters;
using Report.Constructor.Infrastructure.Models.ReportFilters;

namespace Report.Constructor.Infrastructure.ReportConstruction.ReportDataGetters.UsersLiveViews;

internal sealed class UsersLiveViewsDataGetter : IReportDataGetter
{
    public ReportType CompatibleReportType => ReportType.UsersLiveViews;
    public Type CompatibleReportModelType { get; } = typeof(ReportFilterModel);

    private readonly IMapper _mapper;
    private readonly IReportDataRepository _reportDataRepository;

    public UsersLiveViewsDataGetter(IMapper mapper, IReportDataRepository reportDataRepository)
    {
        _mapper = mapper;
        _reportDataRepository = reportDataRepository;
    }

    public async Task<IEnumerable<IReportRow>> GetReportRows(IReportFilter reportFilter)
    {
        var typedCommand = (ReportFilterModel)reportFilter;

        var items = await _reportDataRepository.GetUsersLiveViews(
            typedCommand.StartDateTime,
            typedCommand.EndDateTime,
            typedCommand.UserGroupsIds);


        var rows = _mapper.Map<IEnumerable<UsersLiveViewsReportRow>>(items);

        return rows;
    }
}