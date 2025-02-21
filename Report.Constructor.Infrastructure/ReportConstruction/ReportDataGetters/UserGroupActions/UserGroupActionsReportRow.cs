using Report.Constructor.Infrastructure.Interfaces.ReportConstruction;
using System.ComponentModel;

namespace Report.Constructor.Infrastructure.ReportConstruction.ReportDataGetters.UserGroupActions;

internal sealed class UserGroupActionsReportRow : IReportRow
{
    [DisplayName("Родительская_группа")]
    public string ParentGroup { get; set; } = string.Empty;

    [DisplayName("Группа")]
    public string Group { get; set; } = string.Empty;

    [DisplayName("Кол_во_пользователей")]
    public long? UserCount { get; set; }

    [DisplayName("Удаленных_пользователей")]
    public long DeletedUsers { get; set; }

    [DisplayName("Кол_во_активных_пользователей")]
    public long ActiveUserCount { get; set; }

    [DisplayName("Активность")]
    public long? Activity { get; set; }

    [DisplayName("Кол_во_просмотров")]
    public long? ViewCount { get; set; }

    [DisplayName("Длительностью__мин")]
    public long? DurationInMinutes { get; set; }

    [DisplayName("Уникальных_камер")]
    public long? UniqueCameras { get; set; }

    [DisplayName("Управлений")]
    public long? Controls { get; set; }

    [DisplayName("Просмотров_live")]
    public long? LiveViews { get; set; }

    [DisplayName("Просмотров_АРХИВ")]
    public long? ArchiveViews { get; set; }

    [DisplayName("Заказано_архивов")]
    public long? OrderedArchives { get; set; }

    [DisplayName("Средняя_дневная_аудитория_пользователей")]
    public long? AvgDailyUserAudience { get; set; }

    [DisplayName("Средняя_дневная_длительность_просмотра_live")]
    public long? AvgDailyLiveViewDuration { get; set; }

    [DisplayName("Средняя_дневная_длительность_просмотра_АРХИВ")]
    public long? AvgDailyArchiveViewDuration { get; set; }

    [DisplayName("Длительность_АРХИВ__мин")]
    public long? ArchiveDurationInMinutes { get; set; }

    [DisplayName("Длительность_заказанных_архивов__мин")]
    public long? OrderedArchiveDurationInMinutes { get; set; }

    [DisplayName("Среднее_дневное_кол_во_открытий_камер")]
    public long? AvgDailyCameraOpens { get; set; }

    [DisplayName("Длительность_Live__мин")]
    public long? LiveDurationInMinutes { get; set; }

    [DisplayName("Уникальных_камер_по_управлениям")]
    public long UniqueCamerasByControls { get; set; }
}