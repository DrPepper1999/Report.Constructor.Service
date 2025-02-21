using Report.Constructor.Core.Enums;
using Report.Constructor.Infrastructure.Interfaces.ReportConstruction;
using System.Collections.ObjectModel;
using System.Runtime.Serialization;
using System.Text.Json;
using Report.Constructor.Infrastructure.Interfaces.ReportConstruction.Filters;
using Report.Constructor.Infrastructure.Interfaces.ReportConstruction.Getters;

namespace Report.Constructor.Infrastructure.Utils;

internal sealed class ReportTypeToFilterMap : IReportTypeToFilterMap
{
    private readonly IReportFilterNormalizer _reportFilterNormalizer;
    private readonly IReadOnlyDictionary<ReportType, Type> _typeModelDictionary;

    public ReportTypeToFilterMap(
        IEnumerable<IReportDataGetter> reportDataGetters,
        IEnumerable<IPagedDataGetter> pagedDataGetters,
        IReportFilterNormalizer reportFilterNormalizer)
    {
        _reportFilterNormalizer = reportFilterNormalizer;

        var reportGetterResolvers = reportDataGetters
            .Select(reportDataGetter => new
            {
                ReportType = reportDataGetter.CompatibleReportType,
                CommandType = reportDataGetter.CompatibleReportModelType
            });

        var pagedGetterResolvers = pagedDataGetters
            .Select(reportDataGetter => new
            {
                ReportType = reportDataGetter.CompatibleReportType,
                CommandType = reportDataGetter.CompatibleReportModelType
            });
        
        var dictionary = reportGetterResolvers
            .Concat(pagedGetterResolvers)
            .ToDictionary(x => x.ReportType, x => x.CommandType);
            
        _typeModelDictionary = new ReadOnlyDictionary<ReportType, Type>(dictionary);
    }

    public IFilter BuildFilterForReportType(ReportType reportType, string jsonBody)
    {
        var modelType = _typeModelDictionary[reportType];

        var model = JsonSerializer.Deserialize(jsonBody, modelType);
            
        if (model is null)
        {
            throw new SerializationException("Failed to serialize request to appropriate report params model");
        }

        model = _reportFilterNormalizer.NormalizeFilter((IFilter)model);
        
        return (IFilter)model;
    }
}