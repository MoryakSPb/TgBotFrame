﻿using System.Globalization;
using System.Text;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using TgBotFrame.Commands.Attributes;
using TgBotFrame.Commands.Authorization.Interfaces;
using TgBotFrame.Commands.Authorization.Services;

namespace TgBotFrame.Commands.Authorization.Commands;

[CommandController("Ban")]
public class BanController(
    ITelegramBotClient botClient,
    IAuthorizationData dataContext,
    ReplyUserIdResolver replyUserIdResolver) : CommandControllerBase
{
    [Restricted("admin", "ban_list")]
    [Command(nameof(Ban))]
    public Task Ban() => Ban(TimeSpan.MaxValue, string.Empty);

    [Restricted("admin", "ban_list")]
    [Command(nameof(Ban))]
    public Task Ban(TimeSpan duration) => Ban(duration, string.Empty);

    [Restricted("admin", "ban_list")]
    [Command(nameof(Ban))]
    public Task Ban(string description) => Ban(TimeSpan.MaxValue, description);

    [Restricted("admin", "ban_list")]
    [Command(nameof(Ban))]
    public Task Ban(long userId) => Ban(userId, TimeSpan.MaxValue, string.Empty);

    [Restricted("admin", "ban_list")]
    [Command(nameof(Ban))]
    public Task Ban(long userId, string description) => Ban(userId, TimeSpan.MaxValue, description);

    [Restricted("admin", "ban_list")]
    [Command(nameof(Ban))]
    public Task Ban(long userId, TimeSpan duration) => Ban(userId, duration, string.Empty);

    [Restricted("admin", "ban_list")]
    [Command(nameof(Ban))]
    public async Task Ban(TimeSpan duration, string description)
    {
        long? userId = await replyUserIdResolver.GetReplyUserId(Update, CancellationToken);
        if (userId is null)
        {
            await botClient.SendMessage(
                Context.GetChatId()!,
                ResourceManager.GetString(nameof(BanController_Ban_NotReply), Context.GetCultureInfo())!,
                messageThreadId: Context.GetThreadId(),
                replyParameters: Context.GetMessageId() is not null
                    ? new()
                    {
                        MessageId = Context.GetMessageId()!.Value,
                    }
                    : null, cancellationToken: CancellationToken).ConfigureAwait(false);
            return;
        }

        await Ban(userId.Value, duration, description);
    }

    [Restricted("admin", "ban_list")]
    [Command(nameof(Ban))]
    public async Task Ban(long userId, TimeSpan duration, string description)
    {
        DateTime until = duration == TimeSpan.MaxValue ? DateTime.MaxValue : DateTime.UtcNow + duration;
        DbUser? user = await dataContext.Users.AsNoTracking().FirstOrDefaultAsync(x => x.Id == userId)
            .ConfigureAwait(false);
        if (user is null)
        {
            EntityEntry<DbUser> userEntity = await dataContext.Users.AddAsync(new()
            {
                Id = userId,
            });
            user = userEntity.Entity;
        }

        EntityEntry<DbBan> entity = await dataContext.Bans.AddAsync(new()
        {
            UserId = userId,
            Until = until,
            Description = description,
        }).ConfigureAwait(false);
        await dataContext.SaveChangesAsync(CancellationToken).ConfigureAwait(false);

        int? messageId = Context.GetMessageId();
        string username = user.ToString();
        if (string.IsNullOrWhiteSpace(username))
        {
            username = $@"[{userId:D}]";
        }

        string result;
        if (entity.Entity.Until == DateTime.MaxValue)
        {
            result = string.Format(
                ResourceManager.GetString(nameof(BanController_Ban_ResultForever), Context.GetCultureInfo())!,
                username);
        }
        else
        {
            result = string.Format(
                ResourceManager.GetString(nameof(BanController_Ban_Result), Context.GetCultureInfo())!,
                username,
                entity.Entity.Until);
        }

        await botClient.SendMessage(
            Context.GetChatId()!,
            result,
            messageThreadId: Context.GetThreadId(),
            replyParameters: messageId is not null
                ? new()
                {
                    MessageId = messageId.Value,
                }
                : null, cancellationToken: CancellationToken).ConfigureAwait(false);
    }

    [Restricted("admin", "ban_list")]
    [Command(nameof(UnBan))]
    public async Task UnBan(long userId)
    {
        int removed = await dataContext.Bans.Where(x => x.UserId == userId)
            .ExecuteUpdateAsync(x => x.SetProperty(y => y.Until, DateTime.UtcNow)).ConfigureAwait(false);
        string result;
        if (removed == 0)
        {
            result = ResourceManager.GetString(nameof(BanController_UnBan_NotFound), Context.GetCultureInfo())!;
        }
        else
        {
            DbUser? target = await dataContext.Users.AsNoTracking().FirstOrDefaultAsync(x => x.Id == userId)
                .ConfigureAwait(false);
            result = string.Format(
                ResourceManager.GetString(nameof(BanController_UnBan_Result), Context.GetCultureInfo())!,
                target?.ToString() ?? userId.ToString(@"D"));
        }

        int? messageId = Context.GetMessageId();
        await botClient.SendMessage(
            Context.GetChatId()!,
            result,
            messageThreadId: Context.GetThreadId(),
            replyParameters: messageId is not null
                ? new()
                {
                    MessageId = messageId.Value,
                }
                : null, cancellationToken: CancellationToken).ConfigureAwait(false);
    }

    [Restricted("admin", "ban_list")]
    [Command(nameof(BanInfo))]
    public async Task BanInfo()
    {
        StringBuilder text = new(1024);
        foreach (var bans in dataContext.Bans.Include(x => x.User)
                     .AsNoTracking()
                     .Select(x => new { x.UserId, x.Until, x.User })
                     .GroupBy(x => x.UserId))
        {
            var ban = bans.MaxBy(x => x.Until);
            if (ban is null)
            {
                continue;
            }

            text.Append(ban.User);
            text.Append('\t');
            text.Append('[');
            text.Append(ban.User.Id.ToString(@"D", CultureInfo.InvariantCulture));
            text.Append(']');
            text.AppendLine();
        }

        int? messageId = Context.GetMessageId();
        await botClient.SendMessage(
                Context.GetChatId()!,
                text.Length > 0
                    ? text.ToString()
                    : ResourceManager.GetString(nameof(BanController_BanInfo_Empty), Context.GetCultureInfo())!,
                ParseMode.None,
                messageId is not null
                    ? new()
                    {
                        MessageId = messageId.Value,
                    }
                    : null, messageThreadId: Context.GetThreadId(), cancellationToken: CancellationToken)
            .ConfigureAwait(false);
    }

    [Restricted("admin", "ban_list")]
    [Command(nameof(BanInfo))]
    public async Task BanInfo(long userId)
    {
        DbBan? ban = await dataContext.Bans
            .AsNoTracking()
            .Include(x => x.User)
            .Where(x => x.UserId == userId)
            .OrderByDescending(x => x.Until)
            .FirstOrDefaultAsync().ConfigureAwait(false);
        string result;
        if (ban is not null)
        {
            StringBuilder text = new(1024);
            text.Append(ban.User);
            text.AppendLine();
            text.Append(ResourceManager.GetString(nameof(BanController_BanInfo_Until), Context.GetCultureInfo())!);
            text.Append(ban.Until == DateTime.MaxValue
                ? ResourceManager.GetString(nameof(BanController_BanInfo_Infinite), Context.GetCultureInfo())!
                : ban.Until.ToString(@"G", Context.GetCultureInfo()));
            text.AppendLine();
            text.Append(ban.Description);
            text.AppendLine();
            result = text.ToString();
        }
        else
        {
            result = ResourceManager.GetString(nameof(BanController_BanInfo_NotFound), Context.GetCultureInfo())!;
        }

        int? messageId = Context.GetMessageId();
        await botClient.SendMessage(
            Context.GetChatId()!,
            result,
            messageThreadId: Context.GetThreadId(),
            replyParameters: messageId is not null
                ? new()
                {
                    MessageId = messageId.Value,
                }
                : null, cancellationToken: CancellationToken).ConfigureAwait(false);
    }
}