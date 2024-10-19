namespace TgBotFrame.Commands.Attributes;

[AttributeUsage(AttributeTargets.Method)]
public class CommandAttribute(string name) : Attribute
{
    public string Name { get; } = name;
}