﻿using TgBotFrame.Commands.Attributes;
using TgBotFrame.Commands.Authorization.Imterfaces;

namespace TgBotFrame.Commands.Authorization.Commands;

[CommandController("Roles")]
public class RoleController(ITelegramBotClient botClient, IAuthorizationData dataContext)
    : CommandControllerBase
{
    [Restricted("admin")]
    [Command(nameof(Add) + "Role")]
    public async Task Add(string roleName, long user)
    {
        int? messageId = Context.GetMessageId();
        DbRole? role = await dataContext.Roles.AsNoTracking().FirstOrDefaultAsync(x => x.Name == roleName)
            .ConfigureAwait(false);
        if (role is null)
        {
            await botClient.SendTextMessageAsync(
                Context.GetChatId()!,
                string.Format(
                    ResourceManager.GetString(nameof(RoleManagementController_Add_NotFound), Context.GetCultureInfo())!,
                    roleName),
                Context.GetThreadId(),
                ParseMode.MarkdownV2,
                [],
                replyParameters: messageId is not null
                    ? new()
                    {
                        MessageId = messageId.Value,
                    }
                    : null, cancellationToken: CancellationToken).ConfigureAwait(false);
            return;
        }

        await dataContext.RoleMembers.AddAsync(new()
        {
            UserId = user,
            RoleId = role.Id,
            CreatedBy = Context.GetUserId(),
        }).ConfigureAwait(false);
        await dataContext.SaveChangesAsync().ConfigureAwait(false);

        await botClient.SendTextMessageAsync(
            Context.GetChatId()!,
            string.Format(
                ResourceManager.GetString(nameof(RoleManagementController_Add_Success), Context.GetCultureInfo())!,
                user, roleName),
            Context.GetThreadId(),
            ParseMode.MarkdownV2,
            [],
            replyParameters: messageId is not null
                ? new()
                {
                    MessageId = messageId.Value,
                }
                : null, cancellationToken: CancellationToken).ConfigureAwait(false);
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
            await botClient.SendTextMessageAsync(
                Context.GetChatId()!,
                string.Format(
                    ResourceManager.GetString(nameof(RoleManagementController_Remove_NotFound),
                        Context.GetCultureInfo())!, user, roleName),
                Context.GetThreadId(),
                ParseMode.MarkdownV2,
                [],
                replyParameters: messageId is not null
                    ? new()
                    {
                        MessageId = messageId.Value,
                    }
                    : null, cancellationToken: CancellationToken).ConfigureAwait(false);
            return;
        }

        dataContext.RoleMembers.Remove(roleMember);
        await dataContext.SaveChangesAsync().ConfigureAwait(false);

        await botClient.SendTextMessageAsync(
            Context.GetChatId()!,
            string.Format(
                ResourceManager.GetString(nameof(RoleManagementController_Remove_Success), Context.GetCultureInfo())!,
                user, roleName),
            Context.GetThreadId(),
            ParseMode.MarkdownV2,
            [],
            replyParameters: messageId is not null
                ? new()
                {
                    MessageId = messageId.Value,
                }
                : null, cancellationToken: CancellationToken).ConfigureAwait(false);
    }
}