namespace Report.Constructor.Application.Models.Queries;

public sealed record GetReportInfoQuery
{
    public Guid ReportOrderId { get; set; }
}