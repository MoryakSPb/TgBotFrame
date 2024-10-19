using System.Globalization;
using System.Net;
using System.Threading.RateLimiting;

namespace TgBotFrame.Utility;

public sealed class TelegramRateLimitedHandler : DelegatingHandler
{
    private static readonly FixedWindowRateLimiter _limiter =
        new(new()
        {
            AutoReplenishment = true,
            QueueLimit = 1024,
            QueueProcessingOrder = QueueProcessingOrder.OldestFirst,
            Window = TimeSpan.FromSeconds(1),
            PermitLimit = 30,
        });

    protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request,
        CancellationToken cancellationToken)
    {
        using RateLimitLease lease =
            await _limiter.AcquireAsync(cancellationToken: cancellationToken).ConfigureAwait(false);

        if (lease.IsAcquired) return await base.SendAsync(request, cancellationToken).ConfigureAwait(false);

        HttpResponseMessage response = new(HttpStatusCode.TooManyRequests);
        if (lease.TryGetMetadata(MetadataName.RetryAfter, out TimeSpan retryAfter))
            response.Headers.Add(
                "Retry-After",
                ((int)Math.Floor(retryAfter.TotalSeconds)).ToString(CultureInfo.InvariantCulture));

        return response;
    }
}