using System.Data;
using Dapper;
using Report.Constructor.DAL.ReportOrdersDb.Models.ReportItems;

namespace Report.Constructor.DAL.ReportOrdersDb.Models.UserDefinedTypes;

public static class ParamTableOperationScreenshotJobCrud
{
    public static SqlMapper.ICustomQueryParameter Create(
        IEnumerable<OperationScreenshotJobCrudReportItem>? operationScreenshotJobCrudItems)
    {
        var table = new DataTable();
        table.Columns.Add("UserId", typeof(Guid));
        table.Columns.Add("JobId", typeof(Guid));

        if (operationScreenshotJobCrudItems is null)
        {
            return table.AsTableValuedParameter("[dbo].ParamTableOperationScreenshotJobCrud");
        }
        
        foreach (var value in operationScreenshotJobCrudItems)
        {
            table.Rows.Add(value.UserId, value.JobId);
        }

        return table.AsTableValuedParameter("[dbo].ParamTableOperationScreenshotJobCrud");
    }
}