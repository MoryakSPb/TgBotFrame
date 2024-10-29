using TgBotFrame.Commands.Authorization.Interfaces;
using TgBotFrame.Middleware;

namespace TgBotFrame.Commands.Authorization.Middlewares;

public class BanFilterMiddleware(IAuthorizationData data) : FrameMiddleware
{
    public override async Task InvokeAsync(Update update, FrameContext context, CancellationToken ct = default)
    {
        long? userId = context.GetUserId();
        if (userId is null || !await data.IsUserIdBanned(userId.Value, ct).ConfigureAwait(false))
            await Next(update, context, ct).ConfigureAwait(false);
    }
}