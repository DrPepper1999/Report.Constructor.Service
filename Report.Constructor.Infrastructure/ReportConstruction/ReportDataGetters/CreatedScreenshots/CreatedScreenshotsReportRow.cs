using Report.Constructor.Infrastructure.Interfaces.ReportConstruction;
using System.ComponentModel;

namespace Report.Constructor.Infrastructure.ReportConstruction.ReportDataGetters.CreatedScreenshots;

internal sealed class CreatedScreenshotsReportRow : IReportRow
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

    [DisplayName("кол_во_созданных_расписаний")]
    public long CreatedSchedulesCount { get; set; }

    [DisplayName("кол_во_активных_расписаний")]
    public long ActiveSchedulesCount { get; set; }

    [DisplayName("кол_во_камер_в_новых_расписаниях")]
    public long CamerasInNewSchedulesCount { get; set; }

    [DisplayName("кол_во_камер_в_активных_расписаниях")]
    public long CamerasInActiveSchedulesCount { get; set; }

    [DisplayName("кол_во_снимков_с_активных_расписаний_за_период")]
    public long SnapshotsFromActiveSchedulesInPeriod { get; set; }
}