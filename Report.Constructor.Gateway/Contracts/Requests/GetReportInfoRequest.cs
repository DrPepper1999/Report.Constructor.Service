using Microsoft.AspNetCore.Mvc;

namespace Report.Constructor.Gateway.Contracts.Requests;

internal sealed record GetReportInfoRequest
{
    [FromRoute]
    public Guid ReportOrderId { get; set; }
};