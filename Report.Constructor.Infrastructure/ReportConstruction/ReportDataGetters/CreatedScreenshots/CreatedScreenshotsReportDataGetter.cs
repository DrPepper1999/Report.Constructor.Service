using MapsterMapper;
using Report.Constructor.Core.Enums;
using Report.Constructor.DAL.Interfaces.Repositories;
using Report.Constructor.DAL.ReportOrdersDb.Models.Enums;
using Report.Constructor.Infrastructure.Interfaces.ReportConstruction;
using Report.Constructor.Infrastructure.Interfaces.ReportConstruction.Filters;
using Report.Constructor.Infrastructure.Interfaces.ReportConstruction.Getters;
using Report.Constructor.Infrastructure.Models.ReportFilters;

namespace Report.Constructor.Infrastructure.ReportConstruction.ReportDataGetters.CreatedScreenshots;

internal sealed class CreatedScreenshotsReportDataGetter : IReportDataGetter
{
    public ReportType CompatibleReportType => ReportType.CreatedScreenshots;
    public Type CompatibleReportModelType { get; } = typeof(ReportFilterModel);

    private readonly IAuditRepository _auditRepository;
    private readonly IReportDataRepository _reportDataRepository;
    private readonly IMapper _mapper;

    public CreatedScreenshotsReportDataGetter(
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

        var operationScreenshotJobCrudReportItems = await _auditRepository.GetOperationScreenshotJobCrudData(
                typedCommand.StartDateTime, typedCommand.EndDateTime, CrudOperationType.Create);
        
        var items = await _reportDataRepository.GetCreatedScreenshotsData(
            operationScreenshotJobCrudReportItems,
            typedCommand.StartDateTime,
            typedCommand.EndDateTime,
            typedCommand.UserGroupsIds);

        var rows = _mapper.Map<IEnumerable<CreatedScreenshotsReportRow>>(items);

        return rows;
    }
}