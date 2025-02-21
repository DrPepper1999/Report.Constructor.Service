namespace Report.Constructor.DAL.ReportOrdersDb.Models.ReportItems;

public sealed class ArchiveViewReportItem : ViewActionReportItem
{
    public DateTimeOffset TimeInArchive { get; init; }
}
