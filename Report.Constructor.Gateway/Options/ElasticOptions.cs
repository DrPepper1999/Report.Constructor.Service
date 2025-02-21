namespace Report.Constructor.Gateway.Options;

internal sealed class ElasticOptions
{
    public string Host { get; set; } = default!;
    public string Login { get; set; } = default!;
    public string Password { get; set; } = default!;
    
    public void Deconstruct(out string host, out string login, out string password)
    {
        host = Host;
        login = Login;
        password = Password;
    }
}