using Report.Constructor.Gateway.Configuration.NSwag;
using Report.Constructor.Gateway.Mapper;
using Report.Constructor.Infrastructure.Configuration;

namespace Report.Constructor.Gateway.Configuration;

internal static class ServicesConfiguration
{
    public static void ConfigureServices(this WebApplicationBuilder builder)
    {
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen(c =>
        {
            c.SchemaFilter<NSwagEnumExtensionSchemaFilter>();
        });
        builder.AddInfrastructure();
        builder.Services.AddMapster();
    }
}