namespace Report.Constructor.DAL.ReportOrdersDb.Models.ReportItems;

public sealed class CameraSchedulesReportItem
{
    public DateTime? CreationDate { get; set; }

    public string ServiceType { get; set; } = string.Empty;

    public string Camera { get; set; } = string.Empty;

    public string District { get; set; } = string.Empty;

    public string Address { get; set; } = string.Empty;

    public string FullName { get; set; } = string.Empty;

    public string ADLogin { get; set; } = string.Empty;

    public string SudirLogin { get; set; } = string.Empty;

    public string ParentGroup { get; set; } = string.Empty;

    public string Group { get; set; } = string.Empty;

    public string Mode { get; set; } = string.Empty;

    public string Schedule { get; set; } = string.Empty;

    public string Type { get; set; } = string.Empty;

    public string PresetName { get; set; } = string.Empty;

    public string VNObject { get; set; } = string.Empty;

    public TimeSpan? TransitionTime { get; set; }

    public TimeSpan? ReturnToGDPTime { get; set; }

    public int? DurationInMinutes { get; set; }

    public DateTime? EndDate { get; set; }

    public string ControlLock { get; set; } = string.Empty;
}