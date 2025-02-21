using System.ComponentModel;

namespace Report.Constructor.Core.Enums;

public enum ReportType
{
    [Description("Управление пользователями")]
    UserControlsReport = 2,
    
    [Description("Просмотры пользователями")]
    UsersLiveViews = 3,
    
    [Description("Редактирование пользователями")]
    UserEditsReport = 4,
    
    [Description("Активность пользователей")]
    UserActivitiesReport = 5,
    
    [Description("Управление пользователями по группам")]
    UserManagementsGroupedByGroupsReport = 6,
    
    [Description("Просмотры пользователями по группам")]
    UserViewsGroupedByGroupsReport = 7,
    
    [Description("Количество камер по операторам и статусам")]
    CamerasCountByOperatorsAndStatesReport = 8,
    
    [Description("Действия пользователей ЕЦХД")]
    EchdUserActions = 12,
    
    [Description("Действия группы пользователей ЕЦХД")]
    UserGroupActions = 13,
    
    [Description("Формирование скриншотов")]
    CreatedScreenshots = 14,
    
    [Description("Формирование ссылок")]
    CreatedLinks = 15,
    
    [Description("Статистика по заданиям движения камер")]
    CamerasSchedules = 18,
    
    [Description("Переходы по ссылкам")]
    LinksUsage = 19,
    
    [Description("Статистика по расписаниям скриншотов")]
    SchedulesStatistics = 20,
    
    [Description("Расширенный отчет по статистике просмотров камеры")]
    CameraViewReport = 23,
    
    [Description("Детализация действий пользователя при просмотре архива")]
    ArchiveActivities = 28,

    [Description("Отчет о фактах скачивания архивов")]
    ArchiveDownloads = 29,
    
    [Description("Отчёт об активности пользователей (Просмотр видео)")]
    UserActivitiesWatch = 30,
    
    [Description("Отчёт об активности ВИС")]
    ExternalUserActivities = 42
}