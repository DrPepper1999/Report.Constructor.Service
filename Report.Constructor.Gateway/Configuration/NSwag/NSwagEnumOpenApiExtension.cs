using System.Text.Json;
using Microsoft.OpenApi;
using Microsoft.OpenApi.Interfaces;
using Microsoft.OpenApi.Writers;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace Report.Constructor.Gateway.Configuration.NSwag;

public class NSwagEnumOpenApiExtension : IOpenApiExtension
{
    private readonly SchemaFilterContext _context;
    public NSwagEnumOpenApiExtension(SchemaFilterContext context)
    {
        _context = context;
    }

    public void Write(IOpenApiWriter writer, OpenApiSpecVersion specVersion)
    {
        string[] enums = Enum.GetNames(_context.Type);
        JsonSerializerOptions options = new() { WriteIndented = true };
        string value = JsonSerializer.Serialize(enums, options);
        writer.WriteRaw(value);
    }
}