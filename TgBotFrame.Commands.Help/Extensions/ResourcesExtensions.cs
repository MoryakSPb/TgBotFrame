using System.Globalization;
using System.Numerics;

namespace TgBotFrame.Commands.Help.Extensions;

public static class ResourcesExtensions
{
    public const string COMMAND_DESCRIPTION_PREFIX = "Command_Description_";
    public const string CATEGORY_NAME_PREFIX = "Category_Name_";
    public const string CATEGORY_DESCRIPTION_PREFIX = "Category_Description_";

    internal static string GetFormatText(this Type type, CultureInfo cultureInfo, bool name)
    {
        if (type == typeof(string))
            return ResourceManager.GetString("Text" + (name ? "Name" : "Format"), cultureInfo);
        if (type == typeof(bool))
            return ResourceManager.GetString("Bool" + (name ? "Name" : "Format"), cultureInfo);
        if (type == typeof(char))
            return ResourceManager.GetString("Char" + (name ? "Name" : "Format"), cultureInfo);
        if (type == typeof(TimeSpan))
            return ResourceManager.GetString("Duration" + (name ? "Name" : "Format"),
                cultureInfo);
        if (type == typeof(DateTime))
            return ResourceManager.GetString("DateTime" + (name ? "Name" : "Format"),
                cultureInfo);
        if (type == typeof(DateOnly))
            return ResourceManager.GetString("Date" + (name ? "Name" : "Format"), cultureInfo);
        if (type == typeof(TimeOnly))
            return ResourceManager.GetString("Time" + (name ? "Name" : "Format"), cultureInfo);

        Type[] interfaces = type.GetInterfaces();

        if (interfaces.Any(x => x.IsConstructedGenericType && x.GetGenericTypeDefinition() == typeof(IBinaryInteger<>)))
            return ResourceManager.GetString("Int" + (name ? "Name" : "Format"), cultureInfo);
        if (interfaces.Any(x => x.IsConstructedGenericType && x.GetGenericTypeDefinition() == typeof(IFloatingPoint<>)))
            return ResourceManager.GetString("Float" + (name ? "Name" : "Format"), cultureInfo);

        return "???";
    }
}