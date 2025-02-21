using Report.Constructor.Infrastructure.Interfaces.ReportConstruction;
using System.ComponentModel;

namespace Report.Constructor.Infrastructure.ReportConstruction.ReportDataGetters.CameraSchedules;

internal sealed class CameraSchedulesReportRow : IReportRow
{
    [DisplayName("Дата_создания")]
    public DateTime? CreationDate { get; set; }

    [DisplayName("Тип_услуги")]
    public string ServiceType { get; set; } = string.Empty;

    [DisplayName("Камера")]
    public string Camera { get; set; } = string.Empty;

    [DisplayName("Округ")]
    public string District { get; set; } = string.Empty;

    [DisplayName("Адрес")]
    public string Address { get; set; } = string.Empty;

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

    [DisplayName("Режим")]
    public string Mode { get; set; } = string.Empty;

    [DisplayName("Задание")]
    public string Schedule { get; set; } = string.Empty;

    [DisplayName("Тип")]
    public string Type { get; set; } = string.Empty;

    [DisplayName("Название_пресета")]
    public string PresetName { get; set; } = string.Empty;

    [DisplayName("Объект_ВН")]
    public string VNObject { get; set; } = string.Empty;

    [DisplayName("Время_перехода")]
    public TimeSpan? TransitionTime { get; set; }

    [DisplayName("Время_возврата_в_ГДП")]
    public TimeSpan? ReturnToGDPTime { get; set; }

    [DisplayName("Время_нахождения__мин")]
    public int? DurationInMinutes { get; set; }

    [DisplayName("Дата_окончания")]
    public DateTime? EndDate { get; set; }

    [DisplayName("Блокировка_управления")]
    public string ControlLock { get; set; } = string.Empty;
}