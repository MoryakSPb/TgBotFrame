using Telegram.Bot;
using Telegram.Bot.Types.Enums;
using TgBotFrame.Commands.Attributes;
using TgBotFrame.Commands.Extensions;
using TgBotFrame.Commands.Help;
using TgBotFrame.Commands.Services;

namespace TgBotFrame.Example;

[CommandController("Help")]
public class ExtendedHelpController(ITelegramBotClient botClient, CommandExplorerService commandExplorer)
    : HelpCommandController(botClient, commandExplorer)
{
    private readonly ITelegramBotClient _botClient = botClient;

    [Command(nameof(EchoDouble))]
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