using TgBotFrame.Commands.Attributes;
using TgBotFrame.Commands.Authorization.Interfaces;

namespace TgBotFrame.Commands.Authorization.Commands;

[CommandController("Roles")]
public class RoleController(ITelegramBotClient botClient, IAuthorizationData dataContext)
    : CommandControllerBase
{
    [Restricted("admin")]
    [Command(nameof(Add) + "Role")]
    public async Task Add(string roleName, string user)
    {
        if (user.StartsWith('@'))
        {
            user = user[1..];
        }

        long id = await dataContext.Users.AsNoTracking().Where(x => x.UserName == user).Select(x => x.Id)
            .FirstOrDefaultAsync();
        if (id == 0)
        {
            int? messageId = Context.GetMessageId();
            await botClient.SendMessage(
                Context.GetChatId()!,
                ResourceManager.GetString(nameof(RoleController_Add_UserNotFound), Context.GetCultureInfo())!,
                messageThreadId: Context.GetThreadId(),
                parseMode: ParseMode.None,
                replyParameters: messageId is not null
                    ? new()
                    {
                        MessageId = messageId.Value,
                    }
                    : null, cancellationToken: CancellationToken).ConfigureAwait(false);
        }
        else
        {
            await Add(roleName, id);
        }
    }

    [Restricted("admin")]
    [Command(nameof(Add) + "Role")]
    public async Task Add(string roleName, long user)
    {
        int? messageId = Context.GetMessageId();
        DbRole? role = await dataContext.Roles.AsNoTracking().FirstOrDefaultAsync(x => x.Name == roleName)
            .ConfigureAwait(false);
        if (role is null)
        {
            await botClient.SendMessage(
                Context.GetChatId()!,
                string.Format(
                    ResourceManager.GetString(nameof(RoleManagementController_Add_NotFound),
                        Context.GetCultureInfo())!,
                    roleName),
                messageThreadId: Context.GetThreadId(),
                parseMode: ParseMode.None,
                replyParameters: messageId is not null
                    ? new()
                    {
                        MessageId = messageId.Value,
                    }
                    : null, cancellationToken: CancellationToken).ConfigureAwait(false);
            return;
        }

        if (!await dataContext.RoleMembers.AnyAsync(x => x.RoleId == role.Id && x.UserId == user).ConfigureAwait(false))
        {
            await dataContext.RoleMembers.AddAsync(new()
            {
                UserId = user,
                RoleId = role.Id,
            }).ConfigureAwait(false);
            await dataContext.SaveChangesAsync(CancellationToken).ConfigureAwait(false);
        }

        await botClient.SendMessage(
            Context.GetChatId()!,
            string.Format(
                ResourceManager.GetString(nameof(RoleManagementController_Add_Success), Context.GetCultureInfo())!,
                user, roleName),
            messageThreadId: Context.GetThreadId(),
            parseMode: ParseMode.None,
            replyParameters: messageId is not null
                ? new()
                {
                    MessageId = messageId.Value,
                }
                : null, cancellationToken: CancellationToken).ConfigureAwait(false);
    }

    [Restricted("admin")]
    [Command(nameof(Remove) + "Role")]
    public async Task Remove(string roleName, string user)
    {
        if (user.StartsWith('@'))
        {
            user = user[1..];
        }

        long id = await dataContext.Users.AsNoTracking().Where(x => x.UserName == user).Select(x => x.Id)
            .FirstOrDefaultAsync();
        if (id == 0)
        {
            int? messageId = Context.GetMessageId();
            await botClient.SendMessage(
                Context.GetChatId()!,
                ResourceManager.GetString(nameof(RoleController_Add_UserNotFound), Context.GetCultureInfo())!,
                messageThreadId: Context.GetThreadId(),
                parseMode: ParseMode.None,
                replyParameters: messageId is not null
                    ? new()
                    {
                        MessageId = messageId.Value,
                    }
                    : null, cancellationToken: CancellationToken).ConfigureAwait(false);
        }
        else
        {
            await Remove(roleName, id);
        }
    }

    [Restricted("admin")]
    [Command(nameof(Remove) + "Role")]
    public async Task Remove(string roleName, long user)
    {
        int? messageId = Context.GetMessageId();
        DbRoleMember? roleMember = await dataContext.RoleMembers
            .AsTracking()
            .FirstOrDefaultAsync(x => x.UserId == user && x.Role.Name == roleName).ConfigureAwait(false);

        if (roleMember is null)
        {
            await botClient.SendMessage(
                Context.GetChatId()!,
                string.Format(
                    ResourceManager.GetString(nameof(RoleManagementController_Remove_NotFound),
                        Context.GetCultureInfo())!, user, roleName),
                messageThreadId: Context.GetThreadId(),
                parseMode: ParseMode.None,
                replyParameters: messageId is not null
                    ? new()
                    {
                        MessageId = messageId.Value,
                    }
                    : null, cancellationToken: CancellationToken).ConfigureAwait(false);
            return;
        }

        dataContext.RoleMembers.Remove(roleMember);
        await dataContext.SaveChangesAsync(CancellationToken).ConfigureAwait(false);

        await botClient.SendMessage(
            Context.GetChatId()!,
            string.Format(
                ResourceManager.GetString(nameof(RoleManagementController_Remove_Success),
                    Context.GetCultureInfo())!,
                user, roleName),
            messageThreadId: Context.GetThreadId(),
            parseMode: ParseMode.None,
            replyParameters: messageId is not null
                ? new()
                {
                    MessageId = messageId.Value,
                }
                : null, cancellationToken: CancellationToken).ConfigureAwait(false);
    }
}