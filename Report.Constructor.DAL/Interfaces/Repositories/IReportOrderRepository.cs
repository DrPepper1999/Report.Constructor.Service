using Report.Constructor.Core.Enums;
using Report.Constructor.Core.Models;

namespace Report.Constructor.DAL.Interfaces.Repositories;

public interface IReportOrderRepository
{
    Task<ReportOrder?> GetReportOrderById(Guid id);
    Task<Guid> CreateReportOrder(ReportOrder reportOrder);
    Task CompleteReportOrder(Guid reportOrderId, ReportFileData fileData);
    Task SetFailedStatusToReportOrder(Guid reportOrderId, string errorMessage);
    Task SetProcessingStatusToReportOrder(Guid reportOrderId);
}