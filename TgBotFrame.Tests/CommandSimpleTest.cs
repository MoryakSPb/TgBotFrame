using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Telegram.Bot;
using Telegram.Bot.Types;
using TgBotFrame.Commands.Injection;
using TgBotFrame.Services;
using TgBotFrame.Tests.Controllers;
using TgBotFrame.Tests.Stubs;

namespace TgBotFrame.Tests;

public class CommandSimpleTest
{
    private readonly Random _random = new(1710491186);
    [Fact]
    public async Task IncTest()
    {
        IServiceCollection services = new ServiceCollection();
        services.AddSingleton<ITelegramBotClient, StubTelegramBotClient>();
        services.AddLogging(builder => builder.AddConsole());
        services.AddTgBotFrameCommands(builder =>
        {
            builder.TryAddCommandController<SimpleTestController>();
        });
        ServiceProvider provider = services.BuildServiceProvider(true);
        BotService botService = provider.GetRequiredService<BotService>();
        SimpleTestController.Reset();

        await botService.HandleUpdateAsync(null!, new()
        {
            Message = new()
            {
                Id = _random.Next(),
                Chat = new()
                {
                    Id = _random.Next(),
                },
                Text = "/inc",
            },
        }, CancellationToken.None);

        Assert.Equal(1, SimpleTestController.Count);
    }

    [Fact]
    public async Task NotFoundTest()
    {
        IServiceCollection services = new ServiceCollection();
        services.AddSingleton<ITelegramBotClient, StubTelegramBotClient>();
        services.AddLogging(builder => builder.AddConsole());
        services.AddTgBotFrameCommands(builder =>
        {
            builder.TryAddCommandController<SimpleTestController>();
        });
        ServiceProvider provider = services.BuildServiceProvider(true);
        BotService botService = provider.GetRequiredService<BotService>();
        SimpleTestController.Reset();

        await botService.HandleUpdateAsync(null!, new()
        {
            Message = new()
            {
                Id = _random.Next(),
                Chat = new()
                {
                    Id = _random.Next(),
                },
                Text = "/inc2",
            },
        }, CancellationToken.None);

        StubTelegramBotClient botClient = (StubTelegramBotClient)provider.GetRequiredService<ITelegramBotClient>();
        Assert.Single(botClient.Messages);
    }

    [Fact]
    public async Task IncDecIncTest()
    {
        IServiceCollection services = new ServiceCollection();
        services.AddSingleton<ITelegramBotClient, StubTelegramBotClient>();
        services.AddLogging(builder => builder.AddConsole());
        services.AddTgBotFrameCommands(builder =>
        {
            builder.TryAddCommandController<SimpleTestController>();
        });
        ServiceProvider provider = services.BuildServiceProvider(true);
        BotService botService = provider.GetRequiredService<BotService>();
        SimpleTestController.Reset();

        await botService.HandleUpdateAsync(null!, new()
        {
            Message = new()
            {
                Id = _random.Next(),
                Chat = new()
                {
                    Id = _random.Next(),
                },
                Text = "/inc",
            },
        }, CancellationToken.None);
        await botService.HandleUpdateAsync(null!, new()
        {
            Message = new()
            {
                Id = _random.Next(),
                Chat = new()
                {
                    Id = _random.Next(),
                },
                Text = "/dec",
            },
        }, CancellationToken.None);
        await botService.HandleUpdateAsync(null!, new()
        {
            Message = new()
            {
                Id = _random.Next(),
                Chat = new()
                {
                    Id = _random.Next(),
                },
                Text = "/incTask",
            },
        }, CancellationToken.None);
        await botService.HandleUpdateAsync(null!, new()
        {
            Message = new()
            {
                Id = _random.Next(),
                Chat = new()
                {
                    Id = _random.Next(),
                },
                Text = "/incValueTask",
            },
        }, CancellationToken.None);

        Assert.Equal(2, SimpleTestController.Count);
    }
}