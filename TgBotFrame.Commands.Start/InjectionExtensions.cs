using Microsoft.Extensions.DependencyInjection.Extensions;
using TgBotFrame.Commands.Injection;

namespace TgBotFrame.Commands.Start;

public static class InjectionExtensions
{
    public static FrameCommandsBuilder AddStartCommand(this FrameCommandsBuilder builder, string text)
    {
        builder.ServiceCollection.TryAddSingleton(new StartTextProvider(text));
        builder.TryAddCommandController<StartCommandController>();
        return builder;
    }

    public static FrameCommandsBuilder AddStartCommand(this FrameCommandsBuilder builder, ResourceManager manager,
        string resourceKey)
    {
        builder.ServiceCollection.TryAddSingleton(new StartTextProvider(manager, resourceKey));
        builder.TryAddCommandController<StartCommandController>();
        return builder;
    }
}