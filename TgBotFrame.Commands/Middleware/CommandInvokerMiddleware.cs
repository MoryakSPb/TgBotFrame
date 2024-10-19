using System.Reflection;
using TgBotFrame.Commands.Extensions;
using TgBotFrame.Commands.Services;

namespace TgBotFrame.Commands.Middleware;

public class CommandInvokerMiddleware(CommandsMetricsService metricsService)
    : FrameMiddleware
{
    public override async Task InvokeAsync(Update update, FrameContext context, CancellationToken ct = default)
    {
        MethodInfo? methodInfo = context.GetCommandMethod();
        if (methodInfo is null)
        {
            await Next(update, context, ct).ConfigureAwait(false);
            return;
        }

        CommandControllerBase? controller = context.GetCommandController();
        if (controller is not null)
        {
            controller.Update = update;
            controller.Context = context;
            controller.CancellationToken = ct;
        }

        object?[]? args = context.GetCommandArgs();
        try
        {
            if (methodInfo.ReturnType == typeof(void))
                methodInfo.Invoke(controller, args);
            else if (methodInfo.ReturnType == typeof(Task))
                await ((Task?)methodInfo.Invoke(controller, args) ?? Task.CompletedTask)
                    .ConfigureAwait(false);
            else if (methodInfo.ReturnType == typeof(ValueTask))
                await ((ValueTask?)methodInfo.Invoke(controller, args) ?? ValueTask.CompletedTask)
                    .ConfigureAwait(false);
        }
        finally
        {
            long? userId = context.GetUserId();
            string? commandKey = context.GetCommandName();
            if (userId is not null)
                metricsService.IncCommandsExecuted(commandKey!, userId.Value);
            else
                metricsService.IncCommandsExecuted(commandKey!);
        }
    }
}