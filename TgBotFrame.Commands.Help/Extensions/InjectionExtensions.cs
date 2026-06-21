using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using TgBotFrame.Commands.Help.Services;
using TgBotFrame.Commands.Injection;

namespace TgBotFrame.Commands.Help.Extensions;

public static class InjectionExtensions
{
    public static FrameCommandsBuilder AddHelpCommand(this FrameCommandsBuilder builder)
    {
        builder.TryAddCommandController<HelpCommandController>();
        return builder;
    }

    public static IServiceCollection AddHelpServices(this IServiceCollection serviceCollection)
    {
        serviceCollection.TryAddSingleton<HelpCommandService>();
        return serviceCollection;
    }
}