using Report.Constructor.Infrastructure.Interfaces.ReportConstruction;
using System.ComponentModel;

namespace Report.Constructor.Infrastructure.ReportConstruction.ReportDataGetters.EchdUserActions;

internal sealed class EchdUserActionsReportRow : IReportRow
{
    [DisplayName("Родительская_группа")]
    public string ParentGroup { get; set; } = string.Empty;

    [DisplayName("Группа")]
    public string Group { get; set; } = string.Empty;

    [DisplayName("Статус")]
    public string Status { get; set; } = string.Empty;

    [DisplayName("Логин_AD")]
    public string ADLogin { get; set; } = string.Empty;

    [DisplayName("Логин_базовый")]
    public string BasicLogin { get; set; } = string.Empty;

    [DisplayName("Логин_СУДИР")]
    public string SudirLogin { get; set; } = string.Empty;

    [DisplayName("ФИО")]
    public string FullName { get; set; } = string.Empty;

    [DisplayName("Активных_дней")]
    public long ActiveDays { get; set; }

    [DisplayName("Активность")]
    public long Activity { get; set; }

    [DisplayName("Всего_просмотров")]
    public long? TotalViews { get; set; }

    [DisplayName("Длительностью__мин")]
    public long? DurationInMinutes { get; set; }

    [DisplayName("Уникальных_камер")]
    public long UniqueCameras { get; set; }

    [DisplayName("Управлений")]
    public long Controls { get; set; }

    [DisplayName("Просмотров_Live")]
    public long LiveViews { get; set; }

    [DisplayName("Длительность_Live__мин")]
    public long? LiveDurationInMinutes { get; set; }

    [DisplayName("Просмотров_АРХИВ")]
    public long ArchiveViews { get; set; }

    [DisplayName("Длительность_АРХИВ__мин")]
    public long? ArchiveDurationInMinutes { get; set; }

    [DisplayName("Заказано_архивов")]
    public long OrderedArchives { get; set; }

    [DisplayName("Длительность_заказанных_архивов__мин")]
    public long? OrderedArchiveDurationInMinutes { get; set; }

    [DisplayName("Жалоб_на_портале")]
    public long PortalComplaints { get; set; }

    [DisplayName("Уникальных_камер_по_управлениям")]
    public long UniqueCamerasByControls { get; set; }
}