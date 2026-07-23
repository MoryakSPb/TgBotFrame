using Telegram.Bot.Types.Enums;

namespace TgBotFrame.Options;

public record TgBotOptions
{
    public BotMode BotMode { get; init; } = BotMode.Polling;
    public string? WebhookUrl { get; init; }
    public string? WebhookSecretToken { get; init; }
    public int? WebhookMaxConnections { get; init; }
    public UpdateType[]? AllowedUpdates { get; init; } = Enum.GetValues<UpdateType>();
}