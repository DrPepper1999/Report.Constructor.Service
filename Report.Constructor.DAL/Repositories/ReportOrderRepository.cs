using MapsterMapper;
using Microsoft.EntityFrameworkCore;
using Report.Constructor.Core.Enums;
using Report.Constructor.Core.Models;
using Report.Constructor.DAL.Interfaces.Repositories;
using Report.Constructor.DAL.ReportOrdersDb;
using Report.Constructor.DAL.ReportOrdersDb.Entities;
using WebServiceRestAdapter;
using WebServiceRestAdapter.Models.Requests;
using WebServiceRestAdapter.MoscowVideoRestApi;

namespace Report.Constructor.DAL.Repositories;

internal sealed class ReportOrderRepository : IReportOrderRepository
{
    private readonly ReportOrdersContext _reportOrdersContext;
    private readonly WebServiceRestClient _webServiceRestClient;
    private readonly IMapper _mapper;

    public ReportOrderRepository(ReportOrdersContext reportOrdersContext, IMapper mapper, WebServiceRestClient webServiceRestClient)
    {
        _reportOrdersContext = reportOrdersContext;
        _mapper = mapper;
        _webServiceRestClient = webServiceRestClient;
    }

    public async Task<ReportOrder?> GetReportOrderById(Guid id)
    {
        var reportOrderEntity = await _reportOrdersContext.ReportOrders.FirstOrDefaultAsync(r => r.Id == id);

        if (reportOrderEntity is null)
        {
            return null;
        }
        
        var mapped = _mapper.Map<ReportOrder>(reportOrderEntity);

        return mapped;
    }

    public async Task<Guid> CreateReportOrder(ReportOrder reportOrder)
    {
        var reportOrderEntity = _mapper.Map<ReportOrderEntity>(reportOrder);

        await _reportOrdersContext.ReportOrders.AddAsync(reportOrderEntity);

        await _reportOrdersContext.SaveChangesAsync();

        return reportOrderEntity.Id;
    }

    public async Task CompleteReportOrder(Guid reportOrderId, ReportFileData fileData)
    {
        var reportOrder = await _reportOrdersContext.ReportOrders.FirstAsync(r => r.Id == reportOrderId);

        reportOrder.ReportPath = fileData.ReportPath;
        reportOrder.Status = (int)ReportOrderStatus.Completed;
        
        await _reportOrdersContext.SaveChangesAsync();

        await _webServiceRestClient.UpdateReportState(new UpdateReportStateRequestDto
        {
            ReportId = reportOrderId, 
            ReportState = FileState.Done
        });
    }
    
    public async Task SetFailedStatusToReportOrder(Guid reportOrderId, string errorMessage)
    {
        var reportOrder = await _reportOrdersContext.ReportOrders.FirstAsync(r => r.Id == reportOrderId);

        reportOrder.ErrorMessage = errorMessage;
        reportOrder.Status = (int)ReportOrderStatus.Failed;
        
        await _reportOrdersContext.SaveChangesAsync();

        await _webServiceRestClient.UpdateReportState(new UpdateReportStateRequestDto
        {
            ReportId = reportOrderId, 
            ReportState = FileState.Error,
            ErrorMessage = errorMessage
        });
    }

    public async Task SetProcessingStatusToReportOrder(Guid reportOrderId)
    {
        var reportOrder = await _reportOrdersContext.ReportOrders.FirstAsync(r => r.Id == reportOrderId);

        reportOrder.Status = (int)ReportOrderStatus.Processing;
        
        await _reportOrdersContext.SaveChangesAsync();
    }
}