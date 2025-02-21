namespace Report.Constructor.DAL.ReportOrdersDb.Models.ReportItems;

public sealed class UserGroupActionsReportItem
{
    public string ParentGroup { get; set; } = string.Empty;

    public string Group { get; set; } = string.Empty;

    public long? UserCount { get; set; }

    public long DeletedUsers { get; set; }

    public long ActiveUserCount { get; set; }

    public long? Activity { get; set; }

    public long? ViewCount { get; set; }

    public long? DurationInMinutes { get; set; }

    public long? UniqueCameras { get; set; }

    public long? Controls { get; set; }

    public long? LiveViews { get; set; }

    public long? ArchiveViews { get; set; }

    public long? OrderedArchives { get; set; }

    public long? AvgDailyUserAudience { get; set; }

    public long? AvgDailyLiveViewDuration { get; set; }

    public long? AvgDailyArchiveViewDuration { get; set; }

    public long? ArchiveDurationInMinutes { get; set; }

    public long? OrderedArchiveDurationInMinutes { get; set; }

    public long? AvgDailyCameraOpens { get; set; }

    public long? LiveDurationInMinutes { get; set; }

    public long UniqueCamerasByControls { get; set; }
}