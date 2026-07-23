using System.Collections.Frozen;
using System.Reflection;
using Microsoft.Extensions.Logging;
using Microsoft.FeatureManagement;
using TgBotFrame.Commands.Attributes;
using TgBotFrame.Services;

namespace TgBotFrame.Commands.Services;

public sealed class CommandExplorerService(ILogger<CommandExplorerService> logger)
{
    private FrozenDictionary<string, (FrozenDictionary<string, MethodInfo[]> methods, Assembly[] assemblies)>
        CATEGORIES =
            FrozenDictionary<string, (FrozenDictionary<string, MethodInfo[]> methods, Assembly[] assemblies)>.Empty;

    private FrozenDictionary<string, FrozenDictionary<MethodInfo, ParameterInfo[]>> COMMANDS =
        FrozenDictionary<string, FrozenDictionary<MethodInfo, ParameterInfo[]>>.Empty;

    public async IAsyncEnumerable<KeyValuePair<MethodInfo, ParameterInfo[]>> GetCommand(
        IVariantFeatureManager? featureManager, string commandName)
    {
        if (!COMMANDS.TryGetValue(commandName, out FrozenDictionary<MethodInfo, ParameterInfo[]>? commands))
        {
            yield break;
        }

        if (featureManager is null)
        {
            foreach (KeyValuePair<MethodInfo, ParameterInfo[]> method in commands)
            {
                yield return method;
            }
        }
        else
        {
            foreach (KeyValuePair<MethodInfo, ParameterInfo[]> method in commands)
            {
                if (await BotService.IsFeatureEnabled(featureManager, method.Key).ConfigureAwait(false))
                {
                    yield return method;
                }
            }
        }
    }

    public async IAsyncEnumerable<KeyValuePair<string, IAsyncEnumerable<KeyValuePair<MethodInfo, ParameterInfo[]>>>>
        GetCommands(IVariantFeatureManager? featureManager)
    {
        foreach (KeyValuePair<string, FrozenDictionary<MethodInfo, ParameterInfo[]>> commandName in COMMANDS)
        {
            yield return new(commandName.Key, GetCommand(featureManager, commandName.Key));
        }
    }


    public IReadOnlyCollection<Assembly> GetAssembliesForCategory(in string category)
    {
        return CATEGORIES.TryGetValue(category,
            out (FrozenDictionary<string, MethodInfo[]> methods, Assembly[] assemblies) value)
            ? value.assemblies
            : [];
    }

    public async IAsyncEnumerable<string> GetCategoryCommandsNames(IVariantFeatureManager? featureManager,
        string category)
    {
        if (!CATEGORIES.TryGetValue(category,
                out (FrozenDictionary<string, MethodInfo[]> methods, Assembly[] assemblies) methods))
        {
            yield break;
        }

        foreach (KeyValuePair<string, MethodInfo[]> method in methods.methods)
        {
            foreach (MethodInfo overload in method.Value)
            {
                if (!await BotService.IsFeatureEnabled(featureManager, overload))
                {
                    continue;
                }

                yield return method.Key;
                break;
            }
        }
    }

    public void FillControllers(in IEnumerable<Type> commandControllers)
    {
        Dictionary<string, List<MethodInfo>> commands = [];
        Dictionary<string, (Dictionary<string, List<MethodInfo>> methods, List<Assembly> assemblies)> categories =
            [];

        foreach (Type type in commandControllers.Distinct())
        {
            CommandControllerAttribute? controllerAttribute = type.GetCustomAttribute<CommandControllerAttribute>();
            if (controllerAttribute is null)
            {
                continue;
            }

            Assembly assembly = type.Assembly;


            if (categories.TryGetValue(controllerAttribute.CategoryKey,
                    out (Dictionary<string, List<MethodInfo>> methods, List<Assembly> assemblies) categoryInfo))
            {
                if (!categoryInfo.assemblies.Contains(assembly))
                {
                    categoryInfo.assemblies.Add(assembly);
                }
            }
            else
            {
                categories.Add(controllerAttribute.CategoryKey, ([], [assembly]));
                categoryInfo = categories[controllerAttribute.CategoryKey];
            }

            foreach (MethodInfo methodInfo in type.GetMethods())
            {
                CommandAttribute? attribute = methodInfo.GetCustomAttribute<CommandAttribute>();
                if (attribute is null)
                {
                    continue;
                }

                if (!categoryInfo.methods.TryAdd(attribute.Name, [methodInfo]))
                {
                    categoryInfo.methods[attribute.Name].Add(methodInfo);
                }

                if (!commands.TryAdd(attribute.Name, [methodInfo]))
                {
                    List<MethodInfo> overloads = commands[attribute.Name];
                    if (!overloads.Contains(methodInfo))
                    {
                        overloads.Add(methodInfo);
                    }
                }
            }
        }

        COMMANDS = commands.ToFrozenDictionary(
            x => x.Key,
            x => x.Value.ToFrozenDictionary(
                y => y,
                y => y.GetParameters()),
            StringComparer.OrdinalIgnoreCase);
        CATEGORIES = categories.ToFrozenDictionary(x => x.Key,
            x => (x.Value.methods.ToFrozenDictionary(y => y.Key, y => y.Value.ToArray()),
                x.Value.assemblies.ToArray()));

        logger.LogInformation(@"{all_count:D} overload(s) from {count:D} command(s) loaded",
            COMMANDS.Sum(x => x.Value.Count), COMMANDS.Count);
    }
}