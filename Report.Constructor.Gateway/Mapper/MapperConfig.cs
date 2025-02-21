using AuditReportServiceGenerated;
using Mapster;
using MapsterMapper;
using Report.Constructor.Application.Models.Commands;
using Report.Constructor.Core.Enums;
using Report.Constructor.Core.Models;
using Report.Constructor.DAL.ReportOrdersDb.Models.ReportItems;

namespace Report.Constructor.Gateway.Mapper;

internal static class MapperConfig
{
    public static void AddMapster(this IServiceCollection services)
    {
        var typeAdapterConfig = TypeAdapterConfig.GlobalSettings;

        typeAdapterConfig.NewConfig<CreateReportCommand, ReportOrder>()
            .AfterMapping(order =>
            {
                order.OrderDate = DateTime.UtcNow;
                order.Status = ReportOrderStatus.InQueue;
                order.Id = Guid.NewGuid();
            });
        
        ConfigureAuditReportMappings(typeAdapterConfig);
        
        services.AddSingleton(typeAdapterConfig);
        services.AddScoped<IMapper, MapsterMapper.Mapper>();
    }

    private static void ConfigureAuditReportMappings(TypeAdapterConfig config)
    {
        config
            .NewConfig<GetReportDataResponse, IEnumerable<UserEditsReportItem>>()
            .MapWith(src => src.UserEditsData.Items
                .Select(x => new UserEditsReportItem(x.Date.ToDateTime(), Guid.Parse(x.UserId), x.Count)));
        
        config
            .NewConfig<GetReportDataResponse, IEnumerable<UserControlReportItem>>()
            .MapWith(src => src.UserControlData.Items
                .Select(x => new UserControlReportItem(Guid.Parse(x.UserId), x.Date.ToDateTime(), Guid.Parse(x.CameraId), x.Count)));
        
        config
            .NewConfig<GetReportDataResponse, IEnumerable<UserGroupControlReportItem>>()
            .MapWith(src => src.UserGroupControlData.Items
                .Select(x => new UserGroupControlReportItem(Guid.Parse(x.UserId), x.Date.ToDateTime(), x.Count)));
        
        config
            .NewConfig<GetReportDataResponse, IEnumerable<UserGroupViewsReportItem>>()
            .MapWith(src => src.OperationGetLiveUrlData.Items
                .Select(x => new UserGroupViewsReportItem(Guid.Parse(x.UserId), x.Date.ToDateTime(), x.Count)));
        
        config
            .NewConfig<GetReportDataResponse, IEnumerable<UserViewsReportItem>>()
            .MapWith(src => src.OperationGetLiveUrlData.Items
                .Select(x => new UserViewsReportItem(Guid.Parse(x.UserId), x.Date.ToDateTime(), x.Count)));
        
        config
            .NewConfig<GetReportDataResponse, IEnumerable<OperationPlayerActionReportItem>>()
            .MapWith(src => src.OperationPlayerActionData.Items
                .Select(x => new OperationPlayerActionReportItem(
                    Guid.Parse(x.UserId),
                    Guid.Parse(x.CameraId),
                    x.Action,
                    Guid.Parse(x.ApplicationId),
                    x.Timestamp.ToDateTimeOffset(),
                    x.ActivityType,
                    x.ExtLogin)));
        
        config
            .NewConfig<GetReportDataResponse, IEnumerable<OperationSharedLinkActionReportItem>>()
            .MapWith(src => src.OperationSharedLinkActionData.Items
                .Select(x => new OperationSharedLinkActionReportItem(
                    Guid.Parse(x.Id),
                    Guid.Parse(x.UserId),
                    Guid.Parse(x.CameraId), 
                    x.LinkType,
                    x.ActionType)));

        config
            .NewConfig<GetReportDataResponse, IEnumerable<OperationOrderArchiveReportItem>>()
            .MapWith(src => src.OperationOrderArchiveData.Items
                .Select(x => new OperationOrderArchiveReportItem(
                    x.ArchiveNumber,
                    x.BeginTime.ToDateTimeOffset(),
                    x.EndTime.ToDateTimeOffset(),
                    Guid.Parse(x.UserId),
                    Guid.Parse(x.CameraId),
                    x.Timestamp.ToDateTimeOffset(),
                    x.Reason,
                    Guid.Parse(x.ArchiveTaskId))));

        config
            .NewConfig<GetReportDataResponse, IEnumerable<OperationScreenshotJobCrudReportItem>>()
            .MapWith(src => src.OperationScreenshotJobCrudData.Items
                .Select(x => new OperationScreenshotJobCrudReportItem(
                    Guid.Parse(x.UserId), Guid.Parse(x.JobId))));
        
        config
            .NewConfig<GetReportDataResponse, IEnumerable<ArchiveViewReportItem>>()
            .MapWith(src => src.ArchiveViewData.Items
                .Select(x => new ArchiveViewReportItem
                {
                    TokenId = Guid.Parse(x.TokenId),
                    ApplicationId = Guid.Parse(x.ApplicationId),
                    Action = x.Action,
                    ActionDate = x.ActionDate.ToDateTimeOffset(),
                    ClientIp = x.ClientIp,
                    TimeInArchive = x.TimeInArchive.ToDateTimeOffset()
                }));
        
        config
            .NewConfig<GetReportDataResponse, IEnumerable<LiveViewReportItem>>()
            .MapWith(src => src.LiveViewData.Items
                .Select(x => new LiveViewReportItem
                {
                    TokenId = Guid.Parse(x.TokenId),
                    ApplicationId = Guid.Parse(x.ApplicationId),
                    Action = x.Action,
                    ActionDate = x.ActionDate.ToDateTimeOffset(),
                    ClientIp = x.ClientIp
                }));
    }
}