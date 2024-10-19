namespace TgBotFrame.Commands.RateLimit.Options;

public class RateLimitOptions
{
    public TimeSpan Interval { get; init; } = TimeSpan.FromSeconds(1);
    public int MaxRequestsPerUser { get; init; } = 3;
}