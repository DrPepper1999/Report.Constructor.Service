using MapsterMapper;
using Report.Constructor.Core.Enums;
using Report.Constructor.DAL.Interfaces.Repositories;
using Report.Constructor.Infrastructure.Interfaces.ReportConstruction;
using Report.Constructor.Infrastructure.Interfaces.ReportConstruction.Filters;
using Report.Constructor.Infrastructure.Interfaces.ReportConstruction.Getters;
using Report.Constructor.Infrastructure.Models.ReportFilters;

namespace Report.Constructor.Infrastructure.ReportConstruction.ReportDataGetters.LinksUsage;

internal sealed class LinksUsageReportDataGetter : IReportDataGetter
{
    public ReportType CompatibleReportType => ReportType.LinksUsage;
    public Type CompatibleReportModelType { get; } = typeof(ReportFilterModel);

    private readonly IAuditRepository _auditRepository;
    private readonly IReportDataRepository _reportDataRepository;
    private readonly IMapper _mapper;

    public LinksUsageReportDataGetter(
        IAuditRepository auditRepository,
        IReportDataRepository reportDataRepository,
        IMapper mapper)
    {
        _auditRepository = auditRepository;
        _reportDataRepository = reportDataRepository;
        _mapper = mapper;
    }

    public async Task<IEnumerable<IReportRow>> GetReportRows(IReportFilter reportFilter)
    {
        var typedCommand = (ReportFilterModel)reportFilter;
        
        var operationSharedLinkActionItems = await _auditRepository
            .GetOperationSharedLinkActionData(typedCommand.StartDateTime, typedCommand.EndDateTime);
        
        var items = await _reportDataRepository.GetLinksUsage(
            operationSharedLinkActionItems, typedCommand.UserGroupsIds);
        
        var rows = _mapper.Map<IEnumerable<LinksUsageReportRow>>(items);

        return rows;
    }
}