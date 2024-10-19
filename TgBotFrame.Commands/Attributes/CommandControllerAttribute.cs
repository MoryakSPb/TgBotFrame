namespace TgBotFrame.Commands.Attributes;

[AttributeUsage(AttributeTargets.Class)]
public class CommandControllerAttribute(string categoryKey = "") : Attribute
{
    public string CategoryKey { get; } = categoryKey;
}