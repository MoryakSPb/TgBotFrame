namespace TgBotFrame.Commands;

public class CommandControllerBase
{
    public FrameContext Context { get; internal set; } = null!;
    public Update Update { get; internal set; } = null!;
    public CancellationToken CancellationToken { get; internal set; }
}