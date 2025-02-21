using MapsterMapper;
using MassTransit;
using Report.Constructor.Application.Interfaces.Services;
using Report.Constructor.Application.Models.Commands;
using Report.Constructor.Application.Models.Queries;
using Report.Constructor.Application.Models.Results;
using Report.Constructor.Core.Enums;
using Report.Constructor.Core.Models;
using Report.Constructor.DAL.Interfaces.Repositories;
using Report.Constructor.Infrastructure.Extensions;
using Report.Constructor.Infrastructure.Interfaces.ReportConstruction;
using System.Text.Json;

namespace Report.Constructor.Infrastructure.Services;

internal sealed class ReportService : IReportService
{
    private readonly IBus _bus;
    private readonly IReportOrderRepository _reportOrderRepository;
    private readonly IReportConstructor _reportConstructor;
    private readonly IMapper _mapper;

    public ReportService(IBus bus, IReportOrderRepository reportOrderRepository, IMapper mapper, IReportConstructor reportConstructor)
    {
        _bus = bus;
        _reportOrderRepository = reportOrderRepository;
        _mapper = mapper;
        _reportConstructor = reportConstructor;
    }

    public async Task<Guid> CreateReport(CreateReportCommand createReportCommand)
    {
        var reportOrder = _mapper.Map<ReportOrder>(createReportCommand);
        var orderId = await _reportOrderRepository.CreateReportOrder(reportOrder);

        createReportCommand.OrderId = orderId;
        
        await _bus.Send(createReportCommand);

        return orderId;
    }

    public async Task<ReportOrder?> GetReportInfo(GetReportInfoQuery getReportInfoQuery)
    {
        return await _reportOrderRepository.GetReportOrderById(getReportInfoQuery.ReportOrderId);
    }

    public async Task<GetReportDataResult> GetReportData(GetReportDataQuery getReportDataQuery)
    {
        var reportData = await _reportConstructor.GetReportResultAsync(
            getReportDataQuery.ReportType, getReportDataQuery.ReportFilter);

        var result = new GetReportDataResult
        { 
            ReportRows = reportData.Rows
        };
        
        return result;
    }
    
    public async Task<GetPagedDataResult> GetPagedData(GetReportDataQuery getReportDataQuery)
    {
        var reportData = await _reportConstructor.GetPagedDataAsync(
            getReportDataQuery.ReportType, getReportDataQuery.ReportFilter);

        return new GetPagedDataResult
        { 
            Items = reportData.Items,
            TotalCount = reportData.TotalCount
        };
    }

    public async Task<GetReportFileResult?> GetReportFile(GetReportFileQuery getReportFileQuery)
    {
        var reportOrder = await _reportOrderRepository.GetReportOrderById(getReportFileQuery.ReportOrderId);

        if (reportOrder is null)
        {
            return null;
        }

        if (reportOrder.Status != ReportOrderStatus.Completed || reportOrder.ReportData is null)
        {
            return new GetReportFileResult
            {
                Status = reportOrder.Status,
                ErrorMessage = reportOrder.ErrorMessage
            };
        }
        
        // TODO: Fix this. Need to find proper filter and build name with different logic for different filters
        var deserializedFilter = JsonSerializer.Deserialize<ReportFilter>(reportOrder.ReportFilter);

        if (deserializedFilter is null)
        {
            throw new NotImplementedException();
        }
        
        var extension = Path.GetExtension(reportOrder.ReportData.ReportPath);
        var downloadName = string.Format(
            "{0} c {1:dd.MM.yy} по {2:dd.MM.yy}{3}", 
            reportOrder.ReportType.GetDescription(), 
            deserializedFilter.StartDateTime,
            deserializedFilter.EndDateTime,
            extension);

        var reportData = await File.ReadAllBytesAsync(reportOrder.ReportData.ReportPath);

        return new GetReportFileResult
        {
            DownloadName = downloadName,
            ReportData = reportData, 
            Status = reportOrder.Status
        };
    }

}