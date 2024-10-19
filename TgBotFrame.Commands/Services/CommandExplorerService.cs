using System.Collections.Frozen;
using System.Reflection;
using Microsoft.Extensions.Logging;
using TgBotFrame.Commands.Attributes;

namespace TgBotFrame.Commands.Services;

public sealed class CommandExplorerService(ILogger<CommandExplorerService> logger)
{
    public FrozenDictionary<string, FrozenDictionary<MethodInfo, ParameterInfo[]>> Commands { get; private set; } =
        FrozenDictionary<string, FrozenDictionary<MethodInfo, ParameterInfo[]>>.Empty;

    public FrozenDictionary<string, (Assembly, FrozenSet<string>)> CategoriesCommandsNames { get; private set; } =
        FrozenDictionary<string, (Assembly, FrozenSet<string>)>.Empty;

    public void FillControllers(in IEnumerable<Type> commandControllers)
    {
        Dictionary<string, List<MethodInfo>> controllers = [];
        Dictionary<string, List<string>> categories = [];
        Dictionary<string, Assembly> assemblies = [];
        foreach (Type type in commandControllers.Distinct())
        {
            CommandControllerAttribute? controllerAttribute = type.GetCustomAttribute<CommandControllerAttribute>();
            if (controllerAttribute is null) continue;

            if (!categories.TryGetValue(controllerAttribute.CategoryKey, out List<string>? categoryMethods))
            {
                categoryMethods = [];
                categories.Add(controllerAttribute.CategoryKey, categoryMethods);
                assemblies.Add(controllerAttribute.CategoryKey, type.Assembly);
            }

            foreach (MethodInfo methodInfo in type.GetMethods())
            {
                CommandAttribute? attribute = methodInfo.GetCustomAttribute<CommandAttribute>();
                if (attribute is null) continue;
                string name = attribute.Name;
                categoryMethods.Add(name);
                if (controllers.TryGetValue(name, out List<MethodInfo>? methods))
                {
                    if (!methods.Contains(methodInfo)) methods.Add(methodInfo);
                }
                else
                {
                    controllers.Add(name, [methodInfo]);
                }
            }
        }

        Commands = controllers.ToFrozenDictionary(
            x => x.Key,
            x => x.Value.ToFrozenDictionary(
                y => y,
                y => y.GetParameters()),
            StringComparer.OrdinalIgnoreCase);
        CategoriesCommandsNames = categories.ToFrozenDictionary(x => x.Key,
            x => (assemblies[x.Key], x.Value.Distinct(StringComparer.OrdinalIgnoreCase).ToFrozenSet()));

        logger.LogInformation(@"{all_count:D} overload(s) from {count:D} command(s) loaded",
            Commands.Sum(x => x.Value.Count), Commands.Count);
    }
}