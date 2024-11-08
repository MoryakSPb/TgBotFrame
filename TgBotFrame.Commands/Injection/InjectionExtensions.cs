using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Logging;
using TgBotFrame.Commands.Middleware;
using TgBotFrame.Commands.Services;
using TgBotFrame.Injection;

namespace TgBotFrame.Commands.Injection;

public static class InjectionExtensions
{
    public static IServiceCollection AddTgBotFrameCommands(this IServiceCollection serviceCollection,
        Action<FrameCommandsBuilder> commandBuildAction)
    {
        FrameCommandsBuilder builder = new()
        {
            ServiceCollection = serviceCollection,
        };
        commandBuildAction(builder);

        serviceCollection.AddTgBotFrameCore();
        serviceCollection.TryAddSingleton<CommandsMetricsService>();

        serviceCollection.AddSingleton(provider =>
        {
            ILogger<CommandExplorerService> logger = provider.GetRequiredService<ILogger<CommandExplorerService>>();
            CommandExplorerService explorer = new(logger);

            explorer.FillControllers(builder.Controllers);

            return explorer;
        });

        serviceCollection.TryAddEnumerable(new ServiceDescriptor(typeof(FrameMiddleware),
            typeof(ExceptionCatcherMiddleware), ServiceLifetime.Transient));
        serviceCollection.AddSingleton<FrameMiddleware, ChatInfoMiddleware>();
        serviceCollection.AddTransient<FrameMiddleware, CommandSplitterMiddleware>();
        serviceCollection.AddTransient<FrameMiddleware, CommandRouterMiddleware>();
        foreach (Type middleware in builder.Middlewares)
        {
            serviceCollection.AddTransient(typeof(FrameMiddleware), middleware);
        }

        serviceCollection.AddTransient<FrameMiddleware, CallbackAnswererMiddleware>();
        serviceCollection.AddTransient<FrameMiddleware, CommandInvokerMiddleware>();

        return serviceCollection;
    }
}