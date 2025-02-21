using Microsoft.AspNetCore.Mvc;

namespace Report.Constructor.Gateway.Contracts.Requests;

internal sealed class GetReportFileRequest
{
    [FromRoute]
    public Guid ReportOrderId { get; set; }
}