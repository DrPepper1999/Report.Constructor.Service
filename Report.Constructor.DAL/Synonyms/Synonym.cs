using Report.Constructor.Core.Options;

namespace Report.Constructor.DAL.Synonyms;

internal sealed class Synonym
{
    public string Name { get; set; }
    public string DbHost { get; }
    public string DbName { get; }
    public string DbTable { get; }
    public string Value { get; set; }

    public Synonym(string name, DatabaseOptions databaseOptions, string dbTable)
    {
        Name = name;
        DbHost = databaseOptions.Host;
        DbName = databaseOptions.Name;
        DbTable = dbTable;
        Value = $"[{DbHost}].[{DbName}].[dbo].[{dbTable}]";
    }
}