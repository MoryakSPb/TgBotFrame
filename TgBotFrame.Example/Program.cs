using System.Reflection;
using Microsoft.EntityFrameworkCore;
using OpenTelemetry.Metrics;
using Telegram.Bot;
using TgBotFrame.Commands.Authorization.Extensions;
using TgBotFrame.Commands.Authorization.Interfaces;
using TgBotFrame.Commands.Help.Extensions;
using TgBotFrame.Commands.Injection;
using TgBotFrame.Commands.RateLimit.Middleware;
using TgBotFrame.Commands.RateLimit.Options;
using TgBotFrame.Commands.Start;
using TgBotFrame.Example;
using TgBotFrame.Injection;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

builder.Services.AddOpenTelemetry().WithMetrics(providerBuilder =>
{
    providerBuilder.AddPrometheusExporter();

    providerBuilder.AddMeter(
        "System.Runtime",
        "System.Net.NameResolution",
        "System.Net.Http",
        "Microsoft.Extensions.Diagnostics.ResourceMonitoring",
        "Microsoft.Extensions.Diagnostics.HealthChecks",
        "Microsoft.AspNetCore.Hosting",
        "Microsoft.AspNetCore.Routing",
        "Microsoft.AspNetCore.Diagnostics",
        "Microsoft.AspNetCore.RateLimiting",
        "Microsoft.AspNetCore.HeaderParsing",
        "Microsoft.AspNetCore.Http.Connections",
        "Microsoft.AspNetCore.Server.Kestrel",
        "Microsoft.EntityFrameworkCore", // >= .NET 9
        "TgBotFrame",
        "TgBotFrame.Commands");
});

const string sqliteConnectionString = "Data Source=../data/sqlite/example.sqlite";

string? tgToken = builder.Configuration.GetConnectionString("Telegram");
builder.Services.AddTelegramHttpClient();
builder.Services.AddSingleton<ITelegramBotClient, TelegramBotClient>(provider =>
{
    IHttpClientFactory factory = provider.GetRequiredService<IHttpClientFactory>();
    return new(tgToken!, factory.CreateClient(nameof(ITelegramBotClient)));
});

builder.Services.AddDbContext<ExampleDataContext>(optionsBuilder =>
    optionsBuilder.UseSqlite(sqliteConnectionString));
builder.Services.AddScoped<IAuthorizationData, ExampleDataContext>();

builder.Services.Configure<RateLimitOptions>(builder.Configuration.GetSection("RateLimit"));

builder.Services.AddTgBotFrameCommands(commandsBuilder =>
{
    commandsBuilder.AddStartCommand("Hello, world\\!");
    commandsBuilder.AddHelpCommand();

    commandsBuilder.TryAddCommandMiddleware<RateLimitMiddleware>();
    commandsBuilder.AddAuthorization();

    commandsBuilder.TryAddControllers(Assembly.GetEntryAssembly()!);
});

builder.Services.AddHealthChecks()
    .AddUrlGroup(new Uri("https://api.telegram.org/"), HttpMethod.Head)
    .AddSqlite(sqliteConnectionString);

WebApplication app = builder.Build();

IServiceScopeFactory scopeFactory = app.Services.GetRequiredService<IServiceScopeFactory>();
AsyncServiceScope scope = scopeFactory.CreateAsyncScope();
await using (scope.ConfigureAwait(false))
{
    ExampleDataContext dbContext = scope.ServiceProvider.GetRequiredService<ExampleDataContext>();
    Directory.CreateDirectory("../data/sqlite");
    await dbContext.Database.MigrateAsync().ConfigureAwait(false);
}

app.MapPrometheusScrapingEndpoint();
app.UseHealthChecks("/health");

await app.RunAsync().ConfigureAwait(false);