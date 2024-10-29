using TgBotFrame.Commands.Authorization.Middlewares;
using TgBotFrame.Middleware;

namespace TgBotFrame.Commands.Authorization.Extensions;

public static class FrameContextExtensions
{
    public static string GetUsername(this FrameContext frameContext) =>
        frameContext.Properties.TryGetValue(UserInfoMiddleware.USERNAME_PROPS_KEY, out object? obj) && obj is not null
            ? (string)obj
            : string.Empty;

    public static string GetFirstName(this FrameContext frameContext) =>
        frameContext.Properties.TryGetValue(UserInfoMiddleware.FIRSTNAME_PROPS_KEY, out object? obj) && obj is not null
            ? (string)obj
            : string.Empty;

    public static string GetLastName(this FrameContext frameContext) =>
        frameContext.Properties.TryGetValue(UserInfoMiddleware.LASTNAME_PROPS_KEY, out object? obj) && obj is not null
            ? (string)obj
            : string.Empty;
}