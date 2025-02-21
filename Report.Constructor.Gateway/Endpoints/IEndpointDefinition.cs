namespace Report.Constructor.Gateway.Endpoints;

internal interface IEndpointDefinition
{
    void RegisterEndpoints(WebApplication app);
}