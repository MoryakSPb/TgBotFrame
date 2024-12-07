using System.Text;
using TgBotFrame.Commands.Attributes;
using TgBotFrame.Commands.Authorization.Interfaces;

namespace TgBotFrame.Commands.Authorization.Commands;

[CommandController("Roles")]
public class MentionController(ITelegramBotClient botClient, IAuthorizationData dataContext)
    : CommandControllerBase
{
    [Command(nameof(Mention))]
    public async Task Mention()
    {
        StringBuilder text =
            new(ResourceManager.GetString(nameof(MentionController_Mention_ListTitle), Context.GetCultureInfo())!
                + Environment.NewLine + Environment.NewLine);

        await foreach (var role in dataContext.Roles.Select(x => new { x.Name, x.MentionEnabled }).AsNoTracking()
                           .AsAsyncEnumerable().ConfigureAwait(false))
        {
            text.Append(role.Name);
            text.Append(@": ");
            text.Append(role.MentionEnabled
                ? ResourceManager.GetString(nameof(MentionController_Mention_OnShort), Context.GetCultureInfo())!
                : ResourceManager.GetString(nameof(MentionController_Mention_OffShort), Context.GetCultureInfo())!);
            text.AppendLine();
        }

        int? messageId = Context.GetMessageId();
        await botClient.SendMessage(
            Context.GetChatId()!,
            text.ToString(),
            messageThreadId: Context.GetThreadId(),
            replyParameters: messageId is not null
                ? new()
                {
                    MessageId = messageId.Value,
                }
                : null, cancellationToken: CancellationToken).ConfigureAwait(false);
    }

    [Command(nameof(Mention))]
    public async Task Mention(string roleName)
    {
        int? messageId = Context.GetMessageId();
        DbUser[] users = await dataContext.Roles
            .AsNoTracking()
            .Where(x => x.Name == roleName)
            .Take(1)
            .SelectMany(x => x.Members)
            .ToArrayAsync().ConfigureAwait(false);
        if (users.Length == 0)
        {
            await botClient.SendMessage(
                Context.GetChatId()!,
                ResourceManager.GetString(nameof(MentionController_Mention_NotFound), Context.GetCultureInfo())!,
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


        await botClient.SendMessage(
            Context.GetChatId()!,
            string.Join(@", ", users.Select(x => x.ToString()))
            + ResourceManager.GetString(nameof(MentionController_Mention), Context.GetCultureInfo())!,
            messageThreadId: Context.GetThreadId(),
            parseMode: ParseMode.None,
            cancellationToken: CancellationToken).ConfigureAwait(false);
    }

    [Command(nameof(Mention))]
    [Restricted("admin")]
    public async Task Mention(string roleName, bool enable)
    {
        int? messageId = Context.GetMessageId();
        DbRole? role = await dataContext.Roles.AsTracking().FirstOrDefaultAsync(x => x.Name == roleName)
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

        role.MentionEnabled = enable;
        await dataContext.SaveChangesAsync(CancellationToken).ConfigureAwait(false);

        string text =
            ResourceManager.GetString(nameof(MentionController_Mention_EditResult), Context.GetCultureInfo())!
            + (enable
                ? ResourceManager.GetString(nameof(MentionController_Mention_On), Context.GetCultureInfo())!
                : ResourceManager.GetString(nameof(MentionController_Mention_Off),
                    Context.GetCultureInfo())!);
        await botClient.SendMessage(
            Context.GetChatId()!,
            text,
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