namespace TgBotFrame.Commands.Authorization.Services;

public class ReplyUserIdResolver
{
    public virtual ValueTask<long?> GetReplyUserId(Update update, CancellationToken ct = default) =>
        ValueTask.FromResult(update.Message?.ReplyToMessage?.From?.Id);
}