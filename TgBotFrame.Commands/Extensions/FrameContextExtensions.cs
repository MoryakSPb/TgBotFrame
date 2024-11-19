using System.Globalization;
using System.Reflection;
using TgBotFrame.Commands.Middleware;

namespace TgBotFrame.Commands.Extensions;

public static class FrameContextExtensions
{
    public static string GetBotUsername(this FrameContext frameContext) =>
        frameContext.Properties.TryGetValue(ChatInfoMiddleware.BOT_USERNAME_KEY, out object? obj)
        && obj is not null
            ? (string)obj
            : string.Empty;

    public static long GetBotId(this FrameContext frameContext) =>
        frameContext.Properties.TryGetValue(ChatInfoMiddleware.BOT_ID_KEY, out object? obj) && obj is not null
            ? (long)obj
            : default;

    public static CultureInfo GetCultureInfo(this FrameContext frameContext) =>
        frameContext.Properties.TryGetValue(ChatInfoMiddleware.CULTURE_INFO_PROPS_KEY, out object? obj)
        && obj is not null
            ? (CultureInfo)obj
            : CultureInfo.CurrentUICulture;

    public static string? GetCommandName(this FrameContext frameContext) =>
        frameContext.Properties.TryGetValue(CommandSplitterMiddleware.COMMAND_NAME_KEY, out object? obj)
            ? (string?)obj
            : null;

    public static IList<string> GetCommandArgsRaw(this FrameContext frameContext) =>
        frameContext.Properties.TryGetValue(CommandSplitterMiddleware.COMMAND_ARGS_KEY, out object? obj)
        && obj is not null
            ? (IList<string>)obj
            : [];

    public static long? GetChatId(this FrameContext frameContext) =>
        frameContext.Properties.TryGetValue(ChatInfoMiddleware.CHAT_ID_PROPS_KEY, out object? obj)
            ? (long?)obj
            : null;

    public static int? GetThreadId(this FrameContext frameContext) =>
        frameContext.Properties.TryGetValue(ChatInfoMiddleware.THREAD_ID_PROPS_KEY, out object? obj)
            ? (int?)obj
            : null;

    public static int? GetMessageId(this FrameContext frameContext) =>
        frameContext.Properties.TryGetValue(ChatInfoMiddleware.MESSAGE_ID_PROPS_KEY, out object? obj)
            ? (int?)obj
            : null;

    public static long? GetUserId(this FrameContext frameContext) =>
        frameContext.Properties.TryGetValue(ChatInfoMiddleware.USER_ID_PROPS_KEY, out object? obj)
            ? (long?)obj
            : null;

    public static CommandControllerBase? GetCommandController(this FrameContext frameContext) =>
        frameContext.Properties.TryGetValue(CommandRouterMiddleware.COMMAND_CONTROLLER_KEY, out object? obj)
            ? (CommandControllerBase?)obj
            : null;

    public static MethodInfo? GetCommandMethod(this FrameContext frameContext) =>
        frameContext.Properties.TryGetValue(CommandRouterMiddleware.COMMAND_METHOD_KEY, out object? obj)
            ? (MethodInfo?)obj
            : null;

    public static object?[]? GetCommandArgs(this FrameContext frameContext) =>
        frameContext.Properties.TryGetValue(CommandRouterMiddleware.COMMAND_ARGS_KEY, out object? obj)
        && obj is object?[] { Length: > 0 } array
            ? array
            : null;
}