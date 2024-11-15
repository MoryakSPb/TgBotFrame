using Microsoft.AspNetCore.Mvc;
using TgBotFrame.Commands;
using TgBotFrame.Commands.Attributes;

namespace TgBotFrame.Tests.Controllers;

[CommandController]
public sealed class SimpleTestController : CommandControllerBase
{
    public static int Count { get; private set; } = 0;

    [Command(nameof(Inc))]
    public void Inc() => Count++;

    [Command(nameof(IncTask))]
    public Task IncTask() => Task.CompletedTask.ContinueWith(_ => Count++);

    [Command(nameof(IncValueTask))]
    public ValueTask IncValueTask()
    {
        Count++;
        return ValueTask.CompletedTask;
    }

    [Command(nameof(Dec))]
    public void Dec() => Count--;


    public static void Reset() => Count = 0;
}