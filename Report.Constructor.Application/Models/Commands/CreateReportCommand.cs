using Report.Constructor.Core.Enums;

namespace Report.Constructor.Application.Models.Commands;

public sealed record CreateReportCommand
{
    public required ReportType ReportType { get; set; }
    public required Guid UserId { get; set; }
    public Guid OrderId { get; set; }
    public required string ReportFilter { get; set; }
}