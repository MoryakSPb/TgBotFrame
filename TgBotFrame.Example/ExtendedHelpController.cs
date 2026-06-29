using Microsoft.FeatureManagement;
using Microsoft.FeatureManagement.Mvc;
using Telegram.Bot;
using Telegram.Bot.Types.Enums;
using TgBotFrame.Commands;
using TgBotFrame.Commands.Attributes;
using TgBotFrame.Commands.Extensions;
using TgBotFrame.Commands.Services;

namespace TgBotFrame.Example;

[CommandController("Help")]
public class ExtendedHelpController(
    ITelegramBotClient botClient,
    CommandExplorerService commandExplorer,
    IVariantFeatureManager? featureManager = null)
    : CommandControllerBase //: HelpCommandController(botClient, commandExplorer, featureManager)
{
    private readonly ITelegramBotClient _botClient = botClient;

    [Command(nameof(EchoDouble))]
    [FeatureGate("EchoDoubleFeature")]
    public async Task EchoDouble(string text)
    {
        int? messageId = Context.GetMessageId();
        await _botClient.SendMessage(
            Context.GetChatId()!,
            text + Environment.NewLine + text,
            messageThreadId: Context.GetThreadId(),
            parseMode: ParseMode.None,
            replyParameters: messageId is not null
                ? new()
                {
                    MessageId = messageId.Value,
                }
                : null, cancellationToken: CancellationToken).ConfigureAwait(false);
    }
}