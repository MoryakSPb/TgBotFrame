using Telegram.Bot;
using Telegram.Bot.Types.Enums;
using TgBotFrame.Commands.Attributes;
using TgBotFrame.Commands.Extensions;

namespace TgBotFrame.Commands.Start;

[CommandController(nameof(Start))]
public class StartCommandController(ITelegramBotClient botClient, StartTextProvider startTextProvider)
    : CommandControllerBase
{
    [Command(nameof(Start))]
    public async Task Start()
    {
        int? messageId = Context.GetMessageId();
        await botClient.SendMessage(
            Context.GetChatId()!,
            startTextProvider.GetText(Context.GetCultureInfo()),
            messageThreadId: Context.GetThreadId(),
            parseMode: ParseMode.MarkdownV2,
            replyParameters: messageId is not null
                ? new()
                {
                    MessageId = messageId.Value,
                }
                : null, cancellationToken: CancellationToken).ConfigureAwait(false);
    }
}