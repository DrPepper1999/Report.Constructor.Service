using Report.Constructor.Application.Models.Commands;
using Report.Constructor.Application.Models.Queries;
using Report.Constructor.Application.Models.Results;
using Report.Constructor.Core.Models;

namespace Report.Constructor.Application.Interfaces.Services;

public interface IReportService
{
    Task<Guid> CreateReport(CreateReportCommand createReportCommand);
    Task<GetReportFileResult?> GetReportFile(GetReportFileQuery getReportFileQuery);
    Task<ReportOrder?> GetReportInfo(GetReportInfoQuery getReportInfoQuery);
    Task<GetReportDataResult> GetReportData(GetReportDataQuery getReportDataQuery);
    Task<GetPagedDataResult> GetPagedData(GetReportDataQuery getReportDataQuery);
}