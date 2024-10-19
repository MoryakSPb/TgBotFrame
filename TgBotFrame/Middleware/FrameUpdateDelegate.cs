namespace TgBotFrame.Middleware;

public delegate Task FrameUpdateDelegate(Update update, FrameContext context, CancellationToken ct);