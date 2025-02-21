using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace Report.Constructor.Gateway.Configuration.NSwag;

/// <summary>
/// Источник: https://stackoverflow.com/a/73872112
///
/// Adds extra schema details for an enum in the swagger.json i.e. x-enumNames (used by NSwag to generate Enums for C# client)
/// https://github.com/RicoSuter/NSwag/issues/1234
/// </summary>
public class NSwagEnumExtensionSchemaFilter : ISchemaFilter
{
    public void Apply(OpenApiSchema schema, SchemaFilterContext context)
    {
        if (schema is null)
            throw new ArgumentNullException(nameof(schema));

        if (context is null)
            throw new ArgumentNullException(nameof(context));

        if (context.Type.IsEnum)
            schema.Extensions.Add("x-enumNames", new NSwagEnumOpenApiExtension(context));
    }
}