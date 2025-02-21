using Report.Constructor.Core.Enums;

namespace Report.Constructor.Gateway.Contracts.Responses;

internal sealed record GetReportInfoResponse
{
    public required ReportOrderStatus Status { get; set; }
    
    public required DateTime OrderDate { get; set; }

    public required Guid UserId { get; set; }
}