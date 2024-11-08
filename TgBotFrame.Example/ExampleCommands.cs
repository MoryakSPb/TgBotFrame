using Telegram.Bot;
using Telegram.Bot.Types.Enums;
using TgBotFrame.Commands;
using TgBotFrame.Commands.Attributes;
using TgBotFrame.Commands.Authorization.Attributes;
using TgBotFrame.Commands.Extensions;
using static TgBotFrame.Example.Properties.Resources;

namespace TgBotFrame.Example;

[CommandController]
public class ExampleCommands(ITelegramBotClient botClient) : CommandControllerBase
{
    [Command(nameof(Echo))]
    public async Task Echo(string text)
    {
        int? messageId = Context.GetMessageId();
        await botClient.SendMessage(
            Context.GetChatId()!,
            text,
            messageThreadId: Context.GetThreadId(),
            parseMode: ParseMode.MarkdownV2,
            replyParameters: messageId is not null
                ? new()
                {
                    MessageId = messageId.Value,
                }
                : null, cancellationToken: CancellationToken).ConfigureAwait(false);
    }

    [Command(nameof(GetId))]
    public async Task GetId()
    {
        long? userId = Update.Message?.ReplyToMessage?.From?.Id;
        int? messageId = Context.GetMessageId();
        await botClient.SendMessage(
            Context.GetChatId()!,
            userId is null
                ? ResourceManager.GetString(nameof(ExampleCommands_GetId_NotFound), Context.GetCultureInfo())!
                : userId.Value.ToString("D"),
            messageThreadId: Context.GetThreadId(),
            parseMode: ParseMode.MarkdownV2,
            replyParameters: messageId is not null
                ? new()
                {
                    MessageId = messageId.Value,
                }
                : null, cancellationToken: CancellationToken).ConfigureAwait(false);
    }

    [Command(nameof(Sum))]
    [Restricted("sum")]
    public async Task Sum(decimal left, decimal right)
    {
        int? messageId = Context.GetMessageId();
        await botClient.SendMessage(
            Context.GetChatId()!,
            (left + right).ToString("F"),
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