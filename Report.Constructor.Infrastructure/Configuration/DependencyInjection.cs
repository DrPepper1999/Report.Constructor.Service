using AuditAdapter;
using AuditReportServiceGenerated;
using Grpc.Core;
using Grpc.Net.Client.Configuration;
using MassTransit;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Report.Constructor.Application.Interfaces;
using Report.Constructor.Application.Interfaces.Services;
using Report.Constructor.Application.Models.Commands;
using Report.Constructor.Core.Options;
using Report.Constructor.DAL.Configuration;
using Report.Constructor.Infrastructure.Interfaces;
using Report.Constructor.Infrastructure.Interfaces.ReportConstruction;
using Report.Constructor.Infrastructure.Interfaces.ReportConstruction.Getters;
using Report.Constructor.Infrastructure.Masstransit.Consumers;
using Report.Constructor.Infrastructure.ReportConstruction;
using Report.Constructor.Infrastructure.ReportConstruction.ReportFileBuilders;
using Report.Constructor.Infrastructure.Services;
using Report.Constructor.Infrastructure.Utils;
using WebServiceRestAdapter;

namespace Report.Constructor.Infrastructure.Configuration;

public static class DependencyInjection
{
    public static void AddInfrastructure(this WebApplicationBuilder builder)
    {
        builder.Services.CreateRabbitMq();

        builder.Services.AddTransient<IReportService, ReportService>();

        builder.Services.AddScoped<IReportConstructor, ReportConstructor>();
        builder.Services.AddSingleton<IReportFileBuilder, ExcelReportFileBuilder>();
        builder.Services.AddDataGetters();

        builder.Services.AddScoped<IDbUpdater, DbUpdater>();

        builder.Services.AddHttpClient<WebServiceRestClient>((sp, client) =>
        {
            var options = sp.GetRequiredService<IOptions<ServiceUrlOptions>>().Value;
            client.BaseAddress = new Uri(options.WebServiceRestBaseUrl);
        });

        builder.Services.AddScoped<IReportTypeToFilterMap, ReportTypeToFilterMap>();
        builder.Services.AddScoped<IReportFilterNormalizer, ReportFilterNormalizer>();
        
        builder.Services.AddDal();

        builder.Services
            .AddGrpcClient<AuditReportService.AuditReportServiceClient>((sp, options) =>
            {
                var urlOptions = sp.GetRequiredService<IOptions<ServiceUrlOptions>>().Value;
                options.Address = new Uri(urlOptions.AuditServiceUrl);
            })
            .ConfigureChannel(cfg =>
            {
                cfg.MaxReceiveMessageSize = null;
                cfg.ServiceConfig = new ServiceConfig
                {
                    MethodConfigs =
                    {
                        new MethodConfig
                        {
                            Names = { MethodName.Default },
                            RetryPolicy = new RetryPolicy
                            {
                                MaxAttempts = 3,
                                InitialBackoff = TimeSpan.FromSeconds(5),
                                MaxBackoff = TimeSpan.FromSeconds(30),
                                BackoffMultiplier = 1.5,
                                RetryableStatusCodes = { StatusCode.Unavailable }
                            }
                        }
                    }
                };
            });
        
        builder.Services.AddAuditAdapter();

        builder.Services.AddScoped<ICameraService, CameraService>();
        builder.Services.AddTransient<IApplicationIdDescriptionProvider, ApplicationIdDescriptionProvider>();

        MapperConfiguration.ConfigureMapper();
        
        ClosedXmlConfiguration.ConfigureClosedXml();
    }
    
    private static void CreateRabbitMq(this IServiceCollection services)
    {
        const int connectionTimeOut = 24;
        
        services.AddMassTransit(x =>
        {
            x.AddConsumer<CreateReportCommandConsumer>();

            x.UsingRabbitMq((context, cfg) =>
            {
                var rabbitMqOptions = context.GetRequiredService<IOptions<RabbitMqOptions>>().Value;
                var uri = new Uri($"rabbitmq://{rabbitMqOptions.Host}/");

                cfg.Host(uri, h =>
                {
                    h.RequestedConnectionTimeout(TimeSpan.FromHours(connectionTimeOut));
                    h.Username(rabbitMqOptions.User);
                    h.Password(rabbitMqOptions.Password);
                });
                
                EndpointConvention.Map<CreateReportCommand>(new Uri($"queue:{rabbitMqOptions.CommandsQueueName}"));

                cfg.ReceiveEndpoint(rabbitMqOptions.CommandsQueueName, e =>
                {
                    e.ConfigureConsumer<CreateReportCommandConsumer>(context);
                });
            });
        });
    }

    private static void AddDataGetters(this IServiceCollection services)
    {
        var getterInterfaceTypes = new []
        {
            typeof(IReportDataGetter),
            typeof(IPagedDataGetter)
        };

        foreach (var getterInterfaceType in getterInterfaceTypes)
        {
            typeof(DependencyInjection).Assembly
                .GetTypes()
                .Where(t => t.IsAssignableTo(getterInterfaceType) &&
                            t is { IsAbstract: false, IsInterface: false })
                .ToList()
                .ForEach(t => services.AddScoped(getterInterfaceType, t));
        }
    }
}