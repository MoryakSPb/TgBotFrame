using Telegram.Bot;
using Telegram.Bot.Types.Enums;

namespace TgBotFrame.Commands.Middleware;

public class CallbackAnswererMiddleware(ITelegramBotClient botClient) : FrameMiddleware
{
    public override async Task InvokeAsync(Update update, FrameContext context, CancellationToken ct = default)
    {
        if (update.Type == UpdateType.CallbackQuery)
        {
            await botClient.AnswerCallbackQuery(update.CallbackQuery!.Id, cancellationToken: ct)
                .ConfigureAwait(false);
        }

        await Next(update, context, ct).ConfigureAwait(false);
    }
}