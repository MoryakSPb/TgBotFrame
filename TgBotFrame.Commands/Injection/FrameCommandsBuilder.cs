using System.Reflection;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using TgBotFrame.Commands.Attributes;

namespace TgBotFrame.Commands.Injection;

public class FrameCommandsBuilder
{
    internal List<Type> Controllers { get; } = [];
    internal List<Type> Middlewares { get; } = [];
    public required IServiceCollection ServiceCollection { get; init; }

    public FrameCommandsBuilder TryAddControllers(in Assembly assembly)
    {
        Type[] types = assembly.GetExportedTypes();
        foreach (Type type in types
                     .Where(x => x.GetCustomAttribute<CommandControllerAttribute>() is not null))
        {
            if (!Controllers.Contains(type))
            {
                Controllers.Add(type);
                ServiceCollection.TryAddTransient(type);
            }
        }

        return this;
    }

    public FrameCommandsBuilder TryAddCommandController<T>() where T : CommandControllerBase
    {
        if (!Controllers.Contains(typeof(T)))
        {
            Controllers.Add(typeof(T));
            ServiceCollection.TryAddTransient(typeof(T));
        }

        return this;
    }

    public FrameCommandsBuilder TryAddCommandMiddleware<T>() where T : FrameMiddleware
    {
        if (!Middlewares.Contains(typeof(T)))
        {
            Middlewares.Add(typeof(T));
            ServiceCollection.TryAddTransient(typeof(T));
        }

        return this;
    }
}