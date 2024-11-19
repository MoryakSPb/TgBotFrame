using System.Text.Json.Serialization;

namespace TgBotFrame.Middleware;

/// <summary>
///     Запись для передачи состояния между ПО промежуточного слоя
/// </summary>
public sealed record FrameContext : IDisposable
{
    /// <summary>
    ///     Словарь значений
    /// </summary>
    [JsonExtensionData]
    public IDictionary<string, object?> Properties { get; } = new Dictionary<string, object?>();

    public void Dispose() => Properties.Clear();
}