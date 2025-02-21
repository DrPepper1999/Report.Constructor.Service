using Report.Constructor.Infrastructure.Interfaces.ReportConstruction;
using System.ComponentModel;

namespace Report.Constructor.Infrastructure.ReportConstruction.ReportDataGetters.UserGroupControl;

internal sealed class UserGroupControlReportRow : IReportRow
{
    [DisplayName("Дата")]
    public required string DateString { get; set; }
    
    [DisplayName("Группа")]
    public required string UserGroupName { get; set; }
    
    [DisplayName("Количество_действий")]
    public int ActionCount { get; set; }
}