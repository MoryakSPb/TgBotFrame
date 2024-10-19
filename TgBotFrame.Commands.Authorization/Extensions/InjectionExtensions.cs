using System.Reflection;
using System.Runtime.CompilerServices;
using TgBotFrame.Commands.Authorization.Middlewares;
using TgBotFrame.Commands.Injection;

namespace TgBotFrame.Commands.Authorization.Extensions;

public static class InjectionExtensions
{
    [MethodImpl(MethodImplOptions.NoInlining)]
    public static FrameCommandsBuilder AddAuthorization(this FrameCommandsBuilder builder)
    {
        builder.TryAddControllers(Assembly.GetExecutingAssembly());

        builder.TryAddCommandMiddleware<AuthorizationMiddleware>();

        return builder;
    }
}