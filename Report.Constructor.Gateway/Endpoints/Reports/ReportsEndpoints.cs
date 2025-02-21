using MapsterMapper;
using Report.Constructor.Application.Interfaces.Services;
using Report.Constructor.Application.Models.Commands;
using Report.Constructor.Application.Models.Queries;
using Report.Constructor.Gateway.Contracts.Requests;
using Report.Constructor.Gateway.Contracts.Responses;

namespace Report.Constructor.Gateway.Endpoints.Reports;

internal sealed class ReportsEndpoints : IEndpointDefinition
{
    public void RegisterEndpoints(WebApplication app)
    {
        var group = app.MapGroup("/reports");
        
        group.MapGet("/{reportOrderId:guid}", GetReportInfo)
            .Produces(StatusCodes.Status204NoContent)
            .Produces<GetReportInfoResponse>();

        group.MapPost("/report-data", GetReportData)
            .Produces<GetReportDataResponse>();

        group.MapPost("/paged-data", GetPagedData)
            .Produces<GetPagedDataResponse>();
        
        group.MapPost("/", CreateReport)
            .Produces<Guid>();
    }

    private static async Task<IResult> GetReportInfo([AsParameters] GetReportInfoRequest request, ILogger<ReportsEndpoints> logger,
        IReportService reportService, IMapper mapper)
    {
        var getReportQuery = mapper.Map<GetReportInfoQuery>(request);
        
        var reportOrder = await reportService.GetReportInfo(getReportQuery);

        if (reportOrder is null)
        {
            return Results.NoContent();
        }

        var response = mapper.Map<GetReportInfoResponse>(reportOrder);

        return Results.Ok(response);
    }
    
    private static async Task<IResult> GetReportData(GetReportDataRequest request, IReportService reportService, IMapper mapper)
    {
        var query = mapper.Map<GetReportDataQuery>(request);
        
        var reportData = await reportService.GetReportData(query);

        var mappedResponse = mapper.Map<GetReportDataResponse>(reportData);

        return Results.Ok(mappedResponse);
    }

    private static async Task<IResult> GetPagedData(GetReportDataRequest request, IReportService reportService, IMapper mapper)
    {
        var query = mapper.Map<GetReportDataQuery>(request);
        
        var reportData = await reportService.GetPagedData(query);

        var mappedResponse = mapper.Map<GetPagedDataResponse>(reportData);

        return Results.Ok(mappedResponse);
    }
    
    private static async Task<Guid> CreateReport(
        CreateReportRequest request,
        IReportService reportService, 
        IMapper mapper)
    {
        var command = mapper.Map<CreateReportCommand>(request);
        
        var orderId = await reportService.CreateReport(command);

        return orderId;
    }
}