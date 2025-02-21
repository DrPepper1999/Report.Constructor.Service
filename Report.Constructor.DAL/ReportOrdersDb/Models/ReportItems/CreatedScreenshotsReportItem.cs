namespace Report.Constructor.DAL.ReportOrdersDb.Models.ReportItems;

public sealed class CreatedScreenshotsReportItem
{
    public string ParentGroup { get; set; } = string.Empty;

    public string Group { get; set; } = string.Empty;

    public string Status { get; set; } = string.Empty;

    public string ADLogin { get; set; } = string.Empty;

    public string BasicLogin { get; set; } = string.Empty;

    public string SudirLogin { get; set; } = string.Empty;

    public string FullName { get; set; } = string.Empty;

    public long CreatedSchedulesCount { get; set; }

    public long ActiveSchedulesCount { get; set; }

    public long CamerasInNewSchedulesCount { get; set; }

    public long CamerasInActiveSchedulesCount { get; set; }

    public long SnapshotsFromActiveSchedulesInPeriod { get; set; }
}