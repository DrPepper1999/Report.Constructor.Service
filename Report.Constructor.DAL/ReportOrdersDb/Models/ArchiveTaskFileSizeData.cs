namespace Report.Constructor.DAL.ReportOrdersDb.Models;

public sealed class ArchiveTaskFileSizeData
{
    public Guid ArchiveTaskId { get; set; }
    public int FileSize { get; set; }
}