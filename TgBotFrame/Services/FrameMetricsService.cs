using System.Diagnostics.Metrics;
using Telegram.Bot.Types.Enums;

namespace TgBotFrame.Services;

/// <summary>
/// Сервис, предоставляющий базовые метрики для TgBotFrame
/// </summary>
public sealed class FrameMetricsService : IDisposable
{
    private readonly Meter _meter;

    private readonly Counter<int> _updatesHandled;

    public FrameMetricsService(IMeterFactory meterFactory)
    {
        _meter = meterFactory.Create(@"TgBotFrame");

        _updatesHandled = _meter.CreateCounter<int>(@"updates_handled");
    }

    public void Dispose() => _meter.Dispose();

    /// <summary>
    /// Увеличивает на 1 счетчик обработанных обновлений Telegram
    /// </summary>
    /// <param name="updateType">Тип обновления</param>
    public void IncUpdatesHandled(in UpdateType updateType) =>
        _updatesHandled.Add(
            1,
            new KeyValuePair<string, object?>(@"type", updateType.ToString(@"G")));
}