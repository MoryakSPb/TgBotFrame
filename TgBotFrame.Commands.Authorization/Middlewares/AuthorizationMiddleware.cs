using System.Reflection;
using TgBotFrame.Commands.Authorization.Interfaces;
using TgBotFrame.Middleware;

namespace TgBotFrame.Commands.Authorization.Middlewares;

public class AuthorizationMiddleware(
    IAuthorizationData authorizationDataContext,
    ITelegramBotClient botClient) : FrameMiddleware
{
    public override async Task InvokeAsync(Update update, FrameContext context, CancellationToken ct = default)
    {
        MethodInfo? method = context.GetCommandMethod();
        if (method == null)
        {
            await Next(update, context, ct).ConfigureAwait(false);
            return;
        }

        RestrictedAttribute? attribute = method.GetCustomAttribute<RestrictedAttribute>();
        attribute ??= method.DeclaringType?.GetCustomAttribute<RestrictedAttribute>();
        if (attribute is not null)
        {
            long? userId = context.GetUserId();
            bool allowed = false;
            if (userId is not null)
            {
                IReadOnlyCollection<string> roles = attribute.Roles;
                allowed = await authorizationDataContext.RoleMembers.AsNoTracking().AnyAsync(
                    x => x.UserId == userId && roles.Any(y => x.Role.Name == y), ct).ConfigureAwait(false);
            }

            if (!allowed)
            {
                int? messageId = context.GetMessageId();
                await botClient.SendMessage(
                    context.GetChatId()!,
                    ResourceManager.GetString(nameof(AuthorizationMiddleware_Denied), context.GetCultureInfo())!,
                    messageThreadId: context.GetThreadId(),
                    parseMode: ParseMode.None,
                    replyParameters: messageId is not null
                        ? new()
                        {
                            MessageId = messageId.Value,
                        }
                        : null, cancellationToken: ct).ConfigureAwait(false);
                return;
            }
        }

        await Next(update, context, ct).ConfigureAwait(false);
    }
}