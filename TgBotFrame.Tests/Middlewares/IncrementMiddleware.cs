using Telegram.Bot.Types;
using TgBotFrame.Middleware;

namespace TgBotFrame.Tests.Middlewares;

public class IncrementMiddleware : FrameMiddleware
{
    public int Value { get; private set; }

    public override async Task InvokeAsync(Update update, FrameContext context, CancellationToken ct = default)
    {
        Value++;
        await Next(update, context, ct);
    }
}