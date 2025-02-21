using System.Data;
using Microsoft.Data.SqlClient;

namespace Report.Constructor.DAL.ReportOrdersDb.Models.UserDefinedTypes;

public static class CustomTableTypeParameterFactory
{
    public static SqlParameter Create<T>(string parameterName, string typeName, IEnumerable<T> values)
    {
        var dataTable = new DataTable();

        // маловероятно, но если потребуется использовать кастомный тип в бд, в котором больше одной
        // колонки Value (или они имеют другую конфигурацию) то модифицировать этот метод, чтобы можно было
        // указывать в параметрах эту конфигурацию
        dataTable.Columns.Add(new DataColumn("Value") { AllowDBNull = false });

        foreach (var value in values)
        {
            var row = dataTable.NewRow();
            row[0] = value;
            dataTable.Rows.Add(row);
        }

        return new SqlParameter(parameterName, SqlDbType.Structured)
        {
            TypeName = typeName,
            Value = dataTable
        };
    }
}