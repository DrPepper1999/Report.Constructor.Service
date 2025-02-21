namespace Report.Constructor.Core.Options;

public static class DatabasesOptionsExtensions
{
    public static string ConstructDbConnectionString(this DatabaseOptions options) =>
        $"Server={options.Host};user id={options.UserName};password={options.Password};Database={options.Name};TrustServerCertificate=true";
}
