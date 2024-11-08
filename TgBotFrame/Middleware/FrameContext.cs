using System.Text.Json.Serialization;

namespace TgBotFrame.Middleware;

public sealed record FrameContext : IDisposable
{
    [JsonExtensionData]
    public IDictionary<string, object?> Properties { get; } = new Dictionary<string, object?>();

    public void Dispose() => Properties.Clear();
}