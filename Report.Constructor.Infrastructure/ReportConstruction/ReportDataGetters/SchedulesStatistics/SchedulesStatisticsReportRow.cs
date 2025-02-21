using Report.Constructor.Infrastructure.Interfaces.ReportConstruction;
using System.ComponentModel;

namespace Report.Constructor.Infrastructure.ReportConstruction.ReportDataGetters.SchedulesStatistics;

internal sealed class SchedulesStatisticsReportRow : IReportRow
{
    [DisplayName("Название")]
    public string Name { get; set; } = string.Empty;

    [DisplayName("Дата_создания")]
    public DateTime? CreationDate { get; set; }

    [DisplayName("Дата_окончания")]
    public DateTime? EndDate { get; set; }

    [DisplayName("Активность")]
    public string Activity { get; set; } = string.Empty;

    [DisplayName("ФИО")]
    public string FullName { get; set; } = string.Empty;

    [DisplayName("Логин_AD")]
    public string ADLogin { get; set; } = string.Empty;

    [DisplayName("Логин_СУДИР")]
    public string SudirLogin { get; set; } = string.Empty;

    [DisplayName("Родительская_группа")]
    public string ParentGroup { get; set; } = string.Empty;

    [DisplayName("Группа")]
    public string Group { get; set; } = string.Empty;

    [DisplayName("Группа_камер")]
    public string CameraGroup { get; set; } = string.Empty;

    [DisplayName("Хранение_архива__дней")]
    public int ArchiveStorageDays { get; set; }

    [DisplayName("Периодичность")]
    public string Periodicity { get; set; } = string.Empty;

    [DisplayName("Кол_во_временных_периодов_в_сутки")]
    public int? NumberOfTimePeriodsPerDay { get; set; }

    [DisplayName("Кол_во_камер_в_расписании")]
    public int? NumberOfCamerasInSchedule { get; set; }

    [DisplayName("Кол_во_запрошенных_скриншотов_за_последние_сутки")]
    public int? NumberOfRequestedScreenshotsInLastDay { get; set; }

    [DisplayName("Успешно_подготовлено_скриншотов_за_прошедшие_сутки")]
    public int SuccessfullyPreparedScreenshotsInLastDay { get; set; }
}