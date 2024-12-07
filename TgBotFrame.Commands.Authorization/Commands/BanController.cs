using System.Globalization;
using System.Text;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using TgBotFrame.Commands.Attributes;
using TgBotFrame.Commands.Authorization.Extensions;
using TgBotFrame.Commands.Authorization.Interfaces;

namespace TgBotFrame.Commands.Authorization.Commands;

[CommandController("Ban")]
public class BanController(ITelegramBotClient botClient, IAuthorizationData dataContext) : CommandControllerBase
{
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
    public async Task Ban(long userId, TimeSpan duration, string description)
    {
        DateTime until = duration == TimeSpan.MaxValue ? DateTime.MaxValue : DateTime.UtcNow + duration;
        EntityEntry<DbBan> entity = await dataContext.Bans.AddAsync(new()
        {
            UserId = userId,
            Until = until,
            Description = description,
        }).ConfigureAwait(false);
        await dataContext.SaveChangesAsync(CancellationToken).ConfigureAwait(false);

        int? messageId = Context.GetMessageId();
        await botClient.SendMessage(
            Context.GetChatId()!,
            string.Format(ResourceManager.GetString(nameof(BanController_Ban_Result), Context.GetCultureInfo())!,
                DbUser.GetUserDisplayText(Context.GetUsername(), Context.GetFirstName(), Context.GetLastName()),
                entity.Entity.Until),
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
        DbBan[] entities = await dataContext.Bans.Where(x => x.UserId == userId).AsTracking().ToArrayAsync()
            .ConfigureAwait(false);
        dataContext.Bans.RemoveRange(entities);
        await dataContext.SaveChangesAsync(CancellationToken).ConfigureAwait(false);
        
        int? messageId = Context.GetMessageId();
        DbUser? target = await dataContext.Users.AsNoTracking().FirstOrDefaultAsync(x => x.Id == userId).ConfigureAwait(false);

        await botClient.SendMessage(
            Context.GetChatId()!,
            string.Format(ResourceManager.GetString(nameof(BanController_UnBan_Result), Context.GetCultureInfo())!,
                target?.ToString() ?? userId.ToString(@"D")),
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
        foreach (var user in dataContext.Bans.Include(x => x.User)
                     .AsNoTracking()
                     .GroupBy(x => x.UserId)
                     .Select(x => x.MaxBy(y => y.Until))
                     .Select(x => new { x!.User, x.Until, x.Description }))
        {
            text.Append(user.User);
            text.Append('\t');
            text.Append('[');
            text.Append(user.User.Id.ToString(@"D", CultureInfo.InvariantCulture));
            text.Append(']');
            text.AppendLine();
        }

        int? messageId = Context.GetMessageId();
        await botClient.SendMessage(
                Context.GetChatId()!,
                text.ToString(),
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
        DbBan? ban = dataContext.Bans
            .AsNoTracking()
            .Include(x => x.User)
            .Where(x => x.UserId == userId)
            .MaxBy(x => x.Until);
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