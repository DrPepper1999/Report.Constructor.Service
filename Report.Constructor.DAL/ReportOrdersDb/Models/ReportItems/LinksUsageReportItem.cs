namespace Report.Constructor.DAL.ReportOrdersDb.Models.ReportItems;

public sealed class LinksUsageReportItem
{
    public string ServiceType { get; set; } = string.Empty;

    public string ServiceSubtype { get; set; } = string.Empty;

    public string Camera { get; set; } = string.Empty;

    public string District { get; set; } = string.Empty;

    public string Address { get; set; } = string.Empty;

    public int GeneratedLiveLinks { get; set; }

    public int GeneratedArchiveLinks { get; set; }

    public int LiveLinkClicks { get; set; }

    public int ArchiveLinkClicks { get; set; }

    public int FinishedWithAuthorization { get; set; }
}