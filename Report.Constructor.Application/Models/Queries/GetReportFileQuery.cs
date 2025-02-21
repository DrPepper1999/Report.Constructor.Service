namespace Report.Constructor.Application.Models.Queries;

public record GetReportFileQuery
{
    public Guid ReportOrderId { get; set; }
};