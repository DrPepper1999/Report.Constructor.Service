using Report.Constructor.Infrastructure.Interfaces.ReportConstruction.Filters;

namespace Report.Constructor.Infrastructure.Models.ReportFilters;

public record ReportFilterModel : IReportFilter
{
    public Guid? CameraId { get; set; }
    public DateTime StartDateTime { get; set; }
    public DateTime EndDateTime { get; set; }
    public Guid[] UserGroupsIds { get; set; } = Array.Empty<Guid>();
};