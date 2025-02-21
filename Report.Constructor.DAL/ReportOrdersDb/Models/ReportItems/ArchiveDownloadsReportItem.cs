namespace Report.Constructor.DAL.ReportOrdersDb.Models.ReportItems;

public sealed class ArchiveDownloadsReportItem
{
    public int OrderNumber { get; set; }

    public string CameraName { get; set; } = string.Empty;

    public DateTime Start { get; set; }

    public DateTime End { get; set; }

    public double? FileSizeInGb { get; set; }

    public string FullName { get; set; } = string.Empty;

    public string Action { get; set; } = string.Empty;

    public DateTime DateDownload { get; set; }

    public string Reason { get; set; } = string.Empty;
}