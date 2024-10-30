using System.Diagnostics.Metrics;
using Telegram.Bot.Types.Enums;

namespace TgBotFrame.Services;

public sealed class FrameMetricsService : IDisposable
{
    private readonly Meter _meter;

    private readonly Counter<int> _updatesHandled;

    public FrameMetricsService(IMeterFactory meterFactory)
    {
        _meter = meterFactory.Create(@"TgBotFrame");

        _updatesHandled = _meter.CreateCounter<int>(@"updates_handled");
    }

    public void Dispose()
    {
        _meter.Dispose();
    }

    public void IncUpdatesHandled(in UpdateType updateType) =>
        _updatesHandled.Add(
            1,
            new KeyValuePair<string, object?>(@"type", updateType.ToString(@"G")));
}