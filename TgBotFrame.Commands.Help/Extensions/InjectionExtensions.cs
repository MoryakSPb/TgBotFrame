using TgBotFrame.Commands.Injection;

namespace TgBotFrame.Commands.Help.Extensions;

public static class InjectionExtensions
{
    public static FrameCommandsBuilder AddHelpCommand(this FrameCommandsBuilder builder)
    {
        builder.TryAddCommandController<HelpCommandController>();
        return builder;
    }
}