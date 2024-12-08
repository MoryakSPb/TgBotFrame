using System.Globalization;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Telegram.Bot;
using TgBotFrame.Commands.Help.Extensions;
using TgBotFrame.Commands.Injection;
using TgBotFrame.Services;
using TgBotFrame.Tests.Controllers;
using TgBotFrame.Tests.Properties;
using TgBotFrame.Tests.Stubs;

namespace TgBotFrame.Tests;

public class HelpTests
{
    private readonly Random _random = new(1710491186);

    [Fact]
    public async Task HelpCategoryTest()
    {
        IServiceCollection services = new ServiceCollection();
        services.AddSingleton<ITelegramBotClient, StubTelegramBotClient>();
        services.AddLogging(builder => builder.AddConsole());
        services.AddTgBotFrameCommands(builder =>
        {
            builder.TryAddCommandController<SimpleTestController>();
            builder.AddHelpCommand();
        });
        ServiceProvider provider = services.BuildServiceProvider(true);
        BotService botService = provider.GetRequiredService<BotService>();
        StubTelegramBotClient botClient = (StubTelegramBotClient)provider.GetRequiredService<ITelegramBotClient>();
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
                From = new()
                {
                    Id = _random.NextInt64(),
                    IsBot = false,
                    Username = "test",
                    LanguageCode = "ru",
                },
                Text = "/HelpCategory Simple",
            },
        }, CancellationToken.None);

        Assert.Single(botClient.Messages);
        string excepted = Resources.ResourceManager.GetString(nameof(Resources.Category_Description_Simple),
            CultureInfo.GetCultureInfoByIetfLanguageTag("ru"))!;
        string actual = botClient.Messages.Dequeue().Text;
        Assert.Equal(excepted, actual);
    }
}