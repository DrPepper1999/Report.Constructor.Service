using System.ComponentModel;
using Report.Constructor.DAL.MoscowVideoDb.Enums;
using Report.Constructor.Infrastructure.Interfaces.ReportConstruction;

namespace Report.Constructor.Infrastructure.ReportConstruction.PageGetters.UserActivitiesWatch;

public sealed class UserActivitiesWatchReportRow : IReportRow
{
    public Guid TokenId { get; set; }
    
    public Guid UserId { get; set; }
    public string UserFullName { get; set; }
    public string UserLogin { get; set; }
    
    public string UserGroupName { get; set; }
    
    public Guid CameraId { get; set; }
    public string CameraTitle { get; set; }
    
    public DateTimeOffset? AppealStart { get; set; }
    public DateTimeOffset? AppealEnd { get; set; }

    public VideoAccessType? ActivityType { get; set; }
    public string ActivityTypeName { get; set; }
    
    [Description("Источник обращения IP")]
    public string? AppealSourceIp { get; set; }
    
    [Description("Источник запроса портал")]
    public string? RequestSourcePortal { get; set; }
}