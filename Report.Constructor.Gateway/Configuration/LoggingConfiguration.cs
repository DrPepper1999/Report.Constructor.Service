using Microsoft.Extensions.Options;
using Report.Constructor.Gateway.Options;
using Serilog;
using Serilog.Exceptions;
using Serilog.Sinks.Elasticsearch;

namespace Report.Constructor.Gateway.Configuration;

internal static class LoggingConfiguration
{
    public static void ConfigureLogging(this WebApplicationBuilder builder)
    {
        builder.Services.AddLogging(loggingBuilder => loggingBuilder.ClearProviders());
        
        builder.Host.UseSerilog((ctx, sp, configuration) =>
        {
            var options = sp.GetRequiredService<IOptions<ElasticOptions>>().Value;
            var (host, login, password) = options;

            if (string.IsNullOrWhiteSpace(host) || string.IsNullOrWhiteSpace(login) || string.IsNullOrWhiteSpace(password))
                return;

            var assemblyName = typeof(Program).Assembly.GetName().Name;
            var indexFormat = $"{assemblyName!.ToLower().Replace(".", "-")}-{builder.Environment.EnvironmentName}";
            var elasticOptions = new ElasticsearchSinkOptions(new Uri(host))
            {
                ModifyConnectionSettings = x => x.BasicAuthentication(login, password),
                AutoRegisterTemplate = true,
                IndexFormat = indexFormat
            };

            configuration.Enrich.FromLogContext()
                .Enrich.WithExceptionDetails()
                .Enrich.WithProperty("Environment", builder.Environment.EnvironmentName);
            
            configuration.WriteTo.Console()
                .MinimumLevel.Information();
        });
    }
}