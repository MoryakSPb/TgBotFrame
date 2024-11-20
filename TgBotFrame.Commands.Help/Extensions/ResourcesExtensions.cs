using System.Globalization;
using System.Numerics;
using System.Reflection;
using System.Resources;
using TgBotFrame.Commands.Help.Properties;

namespace TgBotFrame.Commands.Help.Extensions;

public static class ResourcesExtensions
{
    public const string COMMAND_DESCRIPTION_PREFIX = "Command_Description_";
    public const string CATEGORY_NAME_PREFIX = "Category_Name_";
    public const string CATEGORY_DESCRIPTION_PREFIX = "Category_Description_";

    internal static string GetFormatText(this Type type, CultureInfo cultureInfo, bool name)
    {
        if (type == typeof(string))
        {
            return Resources.ResourceManager.GetString(@"Text" + (name ? @"Name" : @"Format"), cultureInfo)!;
        }

        if (type == typeof(bool))
        {
            return Resources.ResourceManager.GetString(@"Bool" + (name ? @"Name" : @"Format"), cultureInfo)!;
        }

        if (type == typeof(char))
        {
            return Resources.ResourceManager.GetString(@"Char" + (name ? @"Name" : @"Format"), cultureInfo)!;
        }

        if (type == typeof(TimeSpan))
        {
            return Resources.ResourceManager.GetString(@"Duration" + (name ? @"Name" : @"Format"),
                cultureInfo)!;
        }

        if (type == typeof(DateTime))
        {
            return Resources.ResourceManager.GetString(@"DateTime" + (name ? @"Name" : @"Format"),
                cultureInfo)!;
        }

        if (type == typeof(DateOnly))
        {
            return Resources.ResourceManager.GetString(@"Date" + (name ? @"Name" : @"Format"), cultureInfo)!;
        }

        if (type == typeof(TimeOnly))
        {
            return Resources.ResourceManager.GetString(@"Time" + (name ? @"Name" : @"Format"), cultureInfo)!;
        }

        Type[] interfaces = type.GetInterfaces();

        if (interfaces.Any(x =>
                x.IsConstructedGenericType && x.GetGenericTypeDefinition() == typeof(IBinaryInteger<>)))
        {
            return Resources.ResourceManager.GetString(@"Int" + (name ? @"Name" : @"Format"), cultureInfo)!;
        }

        if (interfaces.Any(x =>
                x.IsConstructedGenericType && x.GetGenericTypeDefinition() == typeof(IFloatingPoint<>)))
        {
            return Resources.ResourceManager.GetString(@"Float" + (name ? @"Name" : @"Format"), cultureInfo)!;
        }

        return @"???";
    }

    internal static ResourceManager? GetResourceManager(Assembly? source)
    {
        Type[]? types = source?.GetExportedTypes();
        Type? resourcesType = types?.FirstOrDefault(x => x.Name == nameof(Resources));
        PropertyInfo? property = resourcesType?.GetProperty(
            "ResourceManager",
            BindingFlags.GetProperty | BindingFlags.Public | BindingFlags.Static);
        var method = property?.GetMethod;
        var manager = (ResourceManager?)method?.Invoke(null, null);
        return manager;
    }
}