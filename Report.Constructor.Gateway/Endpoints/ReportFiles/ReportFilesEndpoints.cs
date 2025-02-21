using MapsterMapper;
using Microsoft.AspNetCore.Mvc;
using Report.Constructor.Application.Interfaces.Services;
using Report.Constructor.Application.Models.Queries;
using Report.Constructor.Core.Enums;
using Report.Constructor.Gateway.Contracts.Requests;
using Report.Constructor.Gateway.Endpoints.Reports;

namespace Report.Constructor.Gateway.Endpoints.ReportFiles;

internal sealed class ReportFilesEndpoints : IEndpointDefinition
{
    public void RegisterEndpoints(WebApplication app)
    {
        var group = app.MapGroup("/report-files");

        group.MapGet("/{reportOrderId:guid}", GetReportFile)
            .Produces(
                StatusCodes.Status200OK,
                contentType: 
                "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                responseType: typeof(FileStreamResult))
            .Produces(StatusCodes.Status202Accepted)
            .Produces(StatusCodes.Status204NoContent)
            .Produces(StatusCodes.Status400BadRequest);
    }
        
    private static async Task<IResult> GetReportFile([AsParameters] GetReportFileRequest request, ILogger<ReportsEndpoints> logger,
        IReportService reportService, IMapper mapper)
    {
        var getReportQuery = mapper.Map<GetReportFileQuery>(request);
        
        var getReportResult = await reportService.GetReportFile(getReportQuery);

        if (getReportResult is null)
        {
            return Results.NoContent();
        }

        if (getReportResult.Status == ReportOrderStatus.Failed)
        {
            return Results.BadRequest(getReportResult.ErrorMessage);
        }

        if (getReportResult.Status != ReportOrderStatus.Completed)
        {
            return Results.Accepted();
        }

        if (getReportResult.ReportData is null)
        {
            return Results.BadRequest("Cannot get report file data");
        }

        return Results.File(getReportResult.ReportData,
            "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
            getReportResult.DownloadName);
    }
}
