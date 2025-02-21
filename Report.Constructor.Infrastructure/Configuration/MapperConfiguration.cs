using Mapster;
using Report.Constructor.Core.Models;
using Report.Constructor.DAL.ReportOrdersDb.Entities;
using Report.Constructor.DAL.ReportOrdersDb.Models.ReportItems;
using Report.Constructor.Infrastructure.ReportConstruction.ReportDataGetters.CameraView;

namespace Report.Constructor.Infrastructure.Configuration;

internal static class MapperConfiguration
{
    public static void ConfigureMapper()
    {
        var typeAdapterConfig = TypeAdapterConfig.GlobalSettings;

        typeAdapterConfig.NewConfig<ReportOrder, ReportOrderEntity>();

        typeAdapterConfig.NewConfig<ReportOrderEntity, ReportOrder>()
            .Map(dest => dest.ReportData, src => src);

        typeAdapterConfig.NewConfig<CameraViewsStatisticsReportItem, CameraViewStatisticsReportRow>()
            .Map(row => row.UserName, item => item.UserFullName);
    }
}