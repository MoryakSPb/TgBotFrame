using System.Collections.Concurrent;
using System.Threading.RateLimiting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Telegram.Bot.Types;
using TgBotFrame.Commands.Extensions;
using TgBotFrame.Commands.RateLimit.Options;
using TgBotFrame.Middleware;

namespace TgBotFrame.Commands.RateLimit.Middleware;

public class RateLimitMiddleware(IOptions<RateLimitOptions> options, ILogger<RateLimitMiddleware> logger)
    : FrameMiddleware
{
    private static readonly ConcurrentDictionary<long, RateLimiter> _limits = [];

    private readonly FixedWindowRateLimiterOptions _options = new()
    {
        AutoReplenishment = true,
        QueueLimit = 30,
        QueueProcessingOrder = QueueProcessingOrder.OldestFirst,
        Window = options.Value.Interval,
        PermitLimit = options.Value.MaxRequestsPerUser,
    };

    public override async Task InvokeAsync(Update update, FrameContext context, CancellationToken ct = default)
    {
        long userId = context.GetUserId() ?? 0;

        RateLimiter rateLimiter = _limits.GetOrAdd(userId, _ => new FixedWindowRateLimiter(_options));
        using RateLimitLease lease = await rateLimiter.AcquireAsync(cancellationToken: ct).ConfigureAwait(false);

        if (lease.IsAcquired)
            await Next(update, context, ct).ConfigureAwait(false);
        else
            logger.LogInformation("User {id} reached command limit", userId);
    }
}