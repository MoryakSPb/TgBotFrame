using Microsoft.Extensions.Diagnostics.HealthChecks;
using Telegram.Bot;

namespace TgBotFrame.Services;

public sealed class TelegramHealthCheck(ITelegramBotClient botClient) : IHealthCheck
{
    public async Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context,
        CancellationToken cancellationToken = default)
        => await botClient.TestApi(cancellationToken).ConfigureAwait(false)
            ? HealthCheckResult.Healthy()
            : HealthCheckResult.Unhealthy();
}