namespace Report.Constructor.Core.Options;

public class DatabasesOptions
{
    public required DatabaseOptions MoscowVideoDb { get; set; }
    public required DatabaseOptions AggregateTablesDb { get; set; }
    public required DatabaseOptions ReportsDb { get; set; }
    public required DatabaseOptions ReportOrdersDb { get; set; }
    public required DatabaseOptions ArchiveBalancingDb { get; set; }
}

public class DatabaseOptions
{
    public required string Host { get; set; }
    public required string Name { get; set; }
    public string? UserName { get; set; }
    public string? Password { get; set; }
}