namespace Report.Constructor.DAL.ReportOrdersDb.Models.ReportItems;

public sealed class EchdUserActionsReportItem
{
    public string ParentGroup { get; set; } = string.Empty;

    public string Group { get; set; } = string.Empty;

    public string Status { get; set; } = string.Empty;

    public string ADLogin { get; set; } = string.Empty;

    public string BasicLogin { get; set; } = string.Empty;

    public string SudirLogin { get; set; } = string.Empty;

    public string FullName { get; set; } = string.Empty;

    public long ActiveDays { get; set; }

    public long Activity { get; set; }

    public long? TotalViews { get; set; }

    public long? DurationInMinutes { get; set; }

    public long UniqueCameras { get; set; }

    public long Controls { get; set; }

    public long LiveViews { get; set; }

    public long? LiveDurationInMinutes { get; set; }

    public long ArchiveViews { get; set; }

    public long? ArchiveDurationInMinutes { get; set; }

    public long OrderedArchives { get; set; }

    public long? OrderedArchiveDurationInMinutes { get; set; }

    public long PortalComplaints { get; set; }

    public long UniqueCamerasByControls { get; set; }
}