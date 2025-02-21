using Report.Constructor.Gateway.Endpoints;

namespace Report.Constructor.Gateway.Configuration;

internal static class EndpointsMapper
{
    public static void MapEndpoints(this WebApplication app)
    {
        typeof(Program).Assembly
            .GetTypes()
            .Where(t => t.IsAssignableTo(typeof(IEndpointDefinition)) &&
                        t is { IsAbstract: false, IsInterface: false })
            .Select(Activator.CreateInstance)
            .Cast<IEndpointDefinition>()
            .ToList()
            .ForEach(ed => ed.RegisterEndpoints(app));
    }
}