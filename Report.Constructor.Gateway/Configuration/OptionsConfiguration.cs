using Report.Constructor.Core.Options;
using Report.Constructor.Gateway.Options;

namespace Report.Constructor.Gateway.Configuration;

internal static class OptionsConfiguration
{
    public static void ConfigureOptions(this WebApplicationBuilder builder)
    {
        builder.Services.AddOptions<RabbitMqOptions>().Bind(builder.Configuration.GetSection(nameof(RabbitMqOptions)));
        builder.Services.AddOptions<ElasticOptions>().Bind(builder.Configuration.GetSection(nameof(ElasticOptions)));
        builder.Services.AddOptions<DatabasesOptions>().Bind(builder.Configuration.GetSection(nameof(DatabasesOptions)));
        builder.Services.AddOptions<ServiceUrlOptions>().Bind(builder.Configuration.GetSection(nameof(ServiceUrlOptions)));
        builder.Services.AddOptions<ReportsOptions>().Bind(builder.Configuration.GetSection(nameof(ReportsOptions))).ValidateDataAnnotations();
    }
}