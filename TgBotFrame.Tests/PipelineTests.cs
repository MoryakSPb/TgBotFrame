using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Telegram.Bot;
using TgBotFrame.Middleware;
using TgBotFrame.Services;
using TgBotFrame.Tests.Middlewares;
using TgBotFrame.Tests.Stubs;

namespace TgBotFrame.Tests;

public class PipelineTests
{
    [Fact]
    public async Task EmptyPipeline()
    {
        IServiceCollection services = new ServiceCollection();
        services.AddSingleton<ITelegramBotClient, StubTelegramBotClient>();
        services.AddLogging(builder => builder.AddConsole());
        services.AddMetrics();
        services.AddSingleton<FrameMetricsService>();
        services.AddSingleton<BotService>();
        ServiceProvider provider = services.BuildServiceProvider(true);
        BotService botService = provider.GetRequiredService<BotService>();

        await botService.HandleUpdateAsync(null!, new(), CancellationToken.None);
        // ReSharper disable once MethodHasAsyncOverload
        provider.Dispose();
    }


    [Fact]
    public async Task IncPipeline()
    {
        IServiceCollection services = new ServiceCollection();
        services.AddSingleton<IncrementMiddleware>();
        services.AddSingleton<FrameMiddleware, IncrementMiddleware>(x => x.GetRequiredService<IncrementMiddleware>());
        services.AddSingleton<ITelegramBotClient, StubTelegramBotClient>();
        services.AddLogging(builder => builder.AddConsole());
        services.AddMetrics();
        services.AddSingleton<FrameMetricsService>();
        services.AddSingleton<BotService>();
        ServiceProvider provider = services.BuildServiceProvider(true);
        BotService botService = provider.GetRequiredService<BotService>();

        IncrementMiddleware inc = provider.GetRequiredService<IncrementMiddleware>();
        Assert.Equal(0, inc.Value);
        await botService.HandleUpdateAsync(null!, new(), CancellationToken.None);
        Assert.Equal(1, inc.Value);
        await provider.DisposeAsync();
    }

    [Fact]
    public async Task IncDecPipeline()
    {
        IServiceCollection services = new ServiceCollection();
        services.AddSingleton<IncrementMiddleware>();
        services.AddSingleton<FrameMiddleware, IncrementMiddleware>(x => x.GetRequiredService<IncrementMiddleware>());
        services.AddSingleton<DecrementMiddleware>();
        services.AddSingleton<FrameMiddleware, DecrementMiddleware>(x => x.GetRequiredService<DecrementMiddleware>());
        services.AddSingleton<ITelegramBotClient, StubTelegramBotClient>();
        services.AddLogging(builder => builder.AddConsole());
        services.AddMetrics();
        services.AddSingleton<FrameMetricsService>();
        services.AddSingleton<BotService>();
        ServiceProvider provider = services.BuildServiceProvider(true);
        BotService botService = provider.GetRequiredService<BotService>();

        IncrementMiddleware inc = provider.GetRequiredService<IncrementMiddleware>();
        DecrementMiddleware dec = provider.GetRequiredService<DecrementMiddleware>();
        Assert.Equal(0, inc.Value);
        Assert.Equal(0, dec.Value);
        await botService.HandleUpdateAsync(null!, new(), CancellationToken.None);
        Assert.Equal(1, inc.Value);
        Assert.Equal(-1, dec.Value);
        await provider.DisposeAsync();
    }
}