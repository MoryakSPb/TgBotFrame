namespace TgBotFrame.Commands.Authorization.Attributes;

[AttributeUsage(AttributeTargets.Method | AttributeTargets.Class)]
public class RestrictedAttribute : Attribute
{
    public RestrictedAttribute(string role)
    {
        Roles = [role];
    }

    public RestrictedAttribute(params string[] roles)
    {
        Roles = roles;
    }

    public IReadOnlyCollection<string> Roles { get; set; }
}