using Report.Constructor.Gateway.Configuration;

var builder = WebApplication.CreateBuilder(args);

builder.ConfigureOptions();
builder.ConfigureLogging();
builder.ConfigureServices();

var app = builder.Build();

app.ConfigureMiddleware();

await app.MigrateDb();

app.Run();