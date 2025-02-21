using Report.Constructor.DAL.MoscowVideoDb.Enums;
using Report.Constructor.Infrastructure.Interfaces.ReportConstruction.Filters;

namespace Report.Constructor.Infrastructure.Models.ReportFilters;

internal sealed record ExternalUserActivitiesReportFilter : IPageFilter
{
    public required Guid? UserId { get; set; }
    public VideoAccessType? AccessType { get; set; }
    public DateTime Start { get; set; }
    public DateTime End { get; set; }
    public List<string> ExternalLogins { get; set; } = new();
    
    public bool UseExtendedSearch { get; set; }
    public ExtendedCameraSearchFilter? ExtendedSearch { get; set; }
    
    public ICollection<Guid>? SelectedCameraIds { get; set; }

    public required Sorting Sorting { get; set; }
    public Pagination? Pagination { get; set; }
}