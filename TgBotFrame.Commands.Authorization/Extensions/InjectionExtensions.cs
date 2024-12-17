using System.Reflection;
using System.Runtime.CompilerServices;
using Microsoft.Extensions.DependencyInjection.Extensions;
using TgBotFrame.Commands.Authorization.Middlewares;
using TgBotFrame.Commands.Authorization.Services;
using TgBotFrame.Commands.Injection;

namespace TgBotFrame.Commands.Authorization.Extensions;

public static class InjectionExtensions
{
    [MethodImpl(MethodImplOptions.NoInlining)]
    public static FrameCommandsBuilder AddAuthorization(this FrameCommandsBuilder builder)
    {
        builder.ServiceCollection.TryAddScoped<ReplyUserIdResolver>();

        builder.TryAddControllers(Assembly.GetExecutingAssembly());

        builder.TryAddCommandMiddleware<UserInfoMiddleware>();
        builder.TryAddCommandMiddleware<BanFilterMiddleware>();
        builder.TryAddCommandMiddleware<AuthorizationMiddleware>();

        return builder;
    }
}