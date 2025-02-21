namespace Report.Constructor.DAL.ReportOrdersDb.Models.ReportItems;

public sealed class SchedulesStatisticsReportItem
{
    public string Name { get; set; } = string.Empty;

    public DateTime? CreationDate { get; set; }

    public DateTime? EndDate { get; set; }

    public string Activity { get; set; } = string.Empty;

    public string FullName { get; set; } = string.Empty;

    public string ADLogin { get; set; } = string.Empty;

    public string SudirLogin { get; set; } = string.Empty;

    public string ParentGroup { get; set; } = string.Empty;

    public string Group { get; set; } = string.Empty;

    public string CameraGroup { get; set; } = string.Empty;

    public int ArchiveStorageDays { get; set; }

    public string Periodicity { get; set; } = string.Empty;

    public int? NumberOfTimePeriodsPerDay { get; set; }

    public int? NumberOfCamerasInSchedule { get; set; }

    public int? NumberOfRequestedScreenshotsInLastDay { get; set; }

    public int SuccessfullyPreparedScreenshotsInLastDay { get; set; }
}