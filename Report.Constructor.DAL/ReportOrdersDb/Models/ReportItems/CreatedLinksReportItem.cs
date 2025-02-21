namespace Report.Constructor.DAL.ReportOrdersDb.Models.ReportItems;

public sealed class CreatedLinksReportItem
{
    public string ParentGroup { get; set; } = string.Empty;

    public string Group { get; set; } = string.Empty;

    public string Status { get; set; } = string.Empty;

    public string ADLogin { get; set; } = string.Empty;

    public string BasicLogin { get; set; } = string.Empty;

    public string SudirLogin { get; set; } = string.Empty;

    public string FullName { get; set; } = string.Empty;

    public long GeneratedLiveLinksCount { get; set; }

    public long GeneratedArchiveLinksCount { get; set; }

    public long LiveLinkClicks { get; set; }

    public long ArchiveLinkClicks { get; set; }

    public long AuthenticatedLinkClicks { get; set; }

    public long UniqueCamerasCount { get; set; }
}