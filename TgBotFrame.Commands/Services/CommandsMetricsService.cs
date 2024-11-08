using System.Diagnostics.Metrics;

namespace TgBotFrame.Commands.Services;

public sealed class CommandsMetricsService : IDisposable
{
    private readonly Counter<int> _commandsExecuted;
    private readonly Meter _meter;

    public CommandsMetricsService(IMeterFactory meterFactory)
    {
        _meter = meterFactory.Create("TgBotFrame.Commands");

        _commandsExecuted = _meter.CreateCounter<int>("commands_executed");
    }

    public void Dispose() => _meter.Dispose();


    public void IncCommandsExecuted(in string command, in long userId) =>
        _commandsExecuted.Add(
            1,
            new("command", command),
            new("user", userId));

    public void IncCommandsExecuted(in string command) =>
        _commandsExecuted.Add(
            1,
            new KeyValuePair<string, object?>("command", command));
}