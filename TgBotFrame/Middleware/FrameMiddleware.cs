namespace TgBotFrame.Middleware;

public abstract class FrameMiddleware : IDisposable
{
    protected internal FrameUpdateDelegate Next { get; internal set; } = null!;
    public static FrameUpdateDelegate Empty { get; } = (_, _, _) => Task.CompletedTask;

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    public abstract Task InvokeAsync(Update update, FrameContext context, CancellationToken ct = default);

    protected virtual void Dispose(bool disposing)
    {
        if (disposing) Next = null!;
    }
}