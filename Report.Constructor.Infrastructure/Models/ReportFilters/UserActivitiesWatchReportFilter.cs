using Report.Constructor.DAL.MoscowVideoDb.Enums;
using Report.Constructor.Infrastructure.Interfaces.ReportConstruction.Filters;

namespace Report.Constructor.Infrastructure.Models.ReportFilters;

internal sealed record UserActivitiesWatchReportFilter : IPageFilter
{
    public required DateTime DateStart { get; set; }
    public required DateTime DateEnd { get; set; }
    public required Guid? UserId { get; set; }
    public VideoAccessType? ActivityType { get; set; }
    
    public bool UseExtendedSearch { get; set; }
    public ExtendedCameraSearchFilter? ExtendedSearch { get; set; }

    public ICollection<Guid>? SelectedCameraIds { get; set; }


    public required Sorting Sorting { get; set; }
    public Pagination? Pagination { get; set; }
}