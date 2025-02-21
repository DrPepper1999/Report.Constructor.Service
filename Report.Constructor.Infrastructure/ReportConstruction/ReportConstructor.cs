using Report.Constructor.Core.Enums;
using Report.Constructor.Core.Models;
using Report.Constructor.Infrastructure.Interfaces.ReportConstruction;
using Report.Constructor.Infrastructure.Interfaces.ReportConstruction.Filters;
using Report.Constructor.Infrastructure.Interfaces.ReportConstruction.Getters;
using Report.Constructor.Infrastructure.Models.ReportConstruction;

namespace Report.Constructor.Infrastructure.ReportConstruction;

internal sealed class ReportConstructor : IReportConstructor
{
    private readonly IEnumerable<IReportDataGetter> _reportDataGetters;
    private readonly IEnumerable<IPagedDataGetter> _pagedDataGetters;
    private readonly IReportFileBuilder _reportFileBuilder;
    private readonly IReportTypeToFilterMap _reportTypeToFilterMap;

    public ReportConstructor(
        IReportFileBuilder reportFileBuilder, 
        IEnumerable<IReportDataGetter> reportDataGetters, 
        IEnumerable<IPagedDataGetter> pagedDataGetters, 
        IReportTypeToFilterMap reportTypeToFilterMap)
    {
        _reportFileBuilder = reportFileBuilder;
        _reportDataGetters = reportDataGetters;
        _pagedDataGetters = pagedDataGetters;
        _reportTypeToFilterMap = reportTypeToFilterMap;
    }

    public async Task<ReportFileData> ConstructReport(ReportType reportType, string reportFilter)
    {
        var reportResult = await GetReportResultAsync(reportType, reportFilter);
        
        var reportData = ReportData.CreateReportData(reportResult.Rows);

        var createdFileData = await _reportFileBuilder.BuildReportFile(reportData);

        return new ReportFileData
        {
            ReportPath = createdFileData
        };
    }

    public async Task<ReportResult> GetReportResultAsync(ReportType reportType, string reportFilter)
    {
        var filterModel = _reportTypeToFilterMap.BuildFilterForReportType(reportType, reportFilter);

        var reportRows = await GetReportRowsAsync(reportType, filterModel);

        return new ReportResult(reportRows);
    }
    
    public async Task<PagedResult> GetPagedDataAsync(ReportType reportType, string reportFilter)
    {
        var filterModel = _reportTypeToFilterMap.BuildFilterForReportType(reportType, reportFilter);

        var compatibleDataGetter = _pagedDataGetters.First(dg => dg.CompatibleReportType == reportType);
        var result = await compatibleDataGetter.GetPage((IPageFilter)filterModel);

        return result;
    }

    private async Task<IReportRow[]> GetReportRowsAsync(ReportType reportType, IFilter filterModel)
    {
        var compatibleDataGetter = _reportDataGetters.First(dg => dg.CompatibleReportType == reportType);
        var reportRows = await compatibleDataGetter.GetReportRows((IReportFilter)filterModel);
        return reportRows.ToArray();
    }
}