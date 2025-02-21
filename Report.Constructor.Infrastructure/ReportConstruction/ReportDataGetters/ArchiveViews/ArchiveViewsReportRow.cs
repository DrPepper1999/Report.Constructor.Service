using Report.Constructor.Infrastructure.Interfaces.ReportConstruction;

namespace Report.Constructor.Infrastructure.ReportConstruction.ReportDataGetters.ArchiveViews;

public sealed class ArchiveViewsReportRow : IReportRow
{
    public required string Action { get; set; }
    public DateTimeOffset ActionStart { get; set; }
    public DateTimeOffset TimeInArchive { get; set; }
}