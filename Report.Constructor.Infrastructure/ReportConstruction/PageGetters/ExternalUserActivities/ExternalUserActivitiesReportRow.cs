using Report.Constructor.Infrastructure.Interfaces.ReportConstruction;
using System.ComponentModel;

namespace Report.Constructor.Infrastructure.ReportConstruction.PageGetters.ExternalUserActivities;

internal sealed class ExternalUserActivitiesReportRow : IReportRow
{
    [DisplayName("ФИО пользователя")]
    public required string Fio { get; set; }
        
    [DisplayName("Логин пользователя")]
    public required string ExtLogin { get; set; }
        
    [DisplayName("Наименование камеры")]
    public required string Title { get; set; }
        
    [DisplayName("Действие")]
    public required string Action { get; set; }
        
    [DisplayName("Время")]
    public DateTime TimeStamp { get; set; }
        
    [DisplayName("Тип трансляции")]
    public required string ActionType { get; set; }

    [DisplayName("Источник запроса")]
    public required string Portal { get; set; }

    [DisplayName("Идентификатор камеры")]
    public required Guid CameraId { get; set; }

    [DisplayName("Идентификатор пользователя")]
    public required Guid UserId { get; set; }
}