namespace Report.Constructor.Application.Interfaces.Services;

public interface IDbUpdater
{
    Task MigrateDb();
}