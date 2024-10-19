using Microsoft.Extensions.Logging;

namespace TgBotFrame.Commands.Middleware;

public class ExceptionCatcherMiddleware(ILogger<ExceptionCatcherMiddleware> logger) : FrameMiddleware
{
    public override async Task InvokeAsync(Update update, FrameContext context, CancellationToken ct = default)
    {
        try
        {
            await Next(update, context, ct).ConfigureAwait(false);
        }
        catch (OperationCanceledException e)
        {
            logger.LogInformation(e, "Operation canceled");
        }
        catch (Exception e)
        {
            logger.LogError(e, "Exception during update process");
        }
    }
}