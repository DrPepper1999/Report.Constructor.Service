using Report.Constructor.Infrastructure.Interfaces.ReportConstruction;
using System.ComponentModel;

namespace Report.Constructor.Infrastructure.ReportConstruction.ReportDataGetters.UsersLiveViews;

internal sealed class UsersLiveViewsReportRow : IReportRow
{
    [DisplayName("Дата")]
    public required string Date { get; set; }

    [DisplayName("Имя_пользователя")] 
    public required string UserFullName { get; set; }
    
    [DisplayName("Группа")]
    public required string UserGroupName { get; set; }
    
    [DisplayName("Родительская_группа")]
    public string? ParentGroupName { get; set; }
    
    [DisplayName("Количество_просмотров")]
    public int? ViewsCount { get; set; }
}