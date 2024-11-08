using System.Globalization;
using Telegram.Bot;
using Telegram.Bot.Types.Enums;

namespace TgBotFrame.Commands.Middleware;

public class ChatInfoMiddleware(ITelegramBotClient botClient) : FrameMiddleware
{
    public const string CULTURE_INFO_PROPS_KEY = nameof(CultureInfo);
    public const string CHAT_ID_PROPS_KEY = "ChatId";
    public const string THREAD_ID_PROPS_KEY = "ThreadId";
    public const string MESSAGE_ID_PROPS_KEY = "MessageId";
    public const string USER_ID_PROPS_KEY = "UserId";
    public const string BOT_USERNAME_KEY = nameof(BotUsername);

    public string? BotUsername { get; private set; }

    public override async Task InvokeAsync(Update update, FrameContext context, CancellationToken ct = default)
    {
        string? lang = update.Type switch
        {
            UpdateType.Unknown => null,
            UpdateType.Message => update.Message?.From?.LanguageCode,
            UpdateType.InlineQuery => update.InlineQuery?.From.LanguageCode,
            UpdateType.ChosenInlineResult => update.ChosenInlineResult?.From.LanguageCode,
            UpdateType.CallbackQuery => update.CallbackQuery?.From.LanguageCode,
            UpdateType.EditedMessage => update.EditedMessage?.From?.LanguageCode,
            UpdateType.ChannelPost => update.ChannelPost?.From?.LanguageCode,
            UpdateType.EditedChannelPost => update.EditedChannelPost?.From?.LanguageCode,
            UpdateType.ShippingQuery => update.ShippingQuery?.From.LanguageCode,
            UpdateType.PreCheckoutQuery => update.PreCheckoutQuery?.From.LanguageCode,
            UpdateType.Poll => null,
            UpdateType.PollAnswer => update.PollAnswer?.User?.LanguageCode,
            UpdateType.MyChatMember => update.MyChatMember?.From.LanguageCode,
            UpdateType.ChatMember => update.ChatMember?.From.LanguageCode,
            UpdateType.ChatJoinRequest => update.ChatJoinRequest?.From.LanguageCode,
            UpdateType.MessageReaction => update.MessageReaction?.User?.LanguageCode,
            UpdateType.MessageReactionCount => null,
            UpdateType.ChatBoost => null,
            UpdateType.RemovedChatBoost => null,
            UpdateType.BusinessConnection => update.BusinessConnection?.User.LanguageCode,
            UpdateType.BusinessMessage => update.BusinessMessage?.From?.LanguageCode,
            UpdateType.EditedBusinessMessage => update.EditedBusinessMessage?.From?.LanguageCode,
            UpdateType.DeletedBusinessMessages => null,
            UpdateType.PurchasedPaidMedia => update.PurchasedPaidMedia?.From.LanguageCode,
            _ => throw new ArgumentOutOfRangeException(nameof(update)),
        };
        (long? chatId, int? threadId, int? messageId) = update.Type switch
        {
            UpdateType.Unknown => (null, null, null),
            UpdateType.Message => (update.Message!.Chat.Id, update.Message!.MessageThreadId,
                update.Message!.MessageId),
            UpdateType.InlineQuery => (null, null, null),
            UpdateType.ChosenInlineResult => (null, null, null),
            UpdateType.CallbackQuery => (update.CallbackQuery!.Message?.Chat.Id,
                update.CallbackQuery!.Message?.MessageThreadId, update.CallbackQuery!.Message?.MessageId),
            UpdateType.EditedMessage => (update.EditedMessage!.Chat.Id, update.EditedMessage!.MessageThreadId,
                update.EditedMessage!.MessageId),
            UpdateType.ChannelPost => (update.ChannelPost!.Chat.Id, update.ChannelPost!.MessageThreadId,
                update.ChannelPost!.MessageId),
            UpdateType.EditedChannelPost => (update.EditedChannelPost!.Chat.Id,
                update.EditedChannelPost!.MessageThreadId, update.EditedChannelPost!.MessageId),
            UpdateType.ShippingQuery => (null, null, null),
            UpdateType.PreCheckoutQuery => (null, null, null),
            UpdateType.Poll => (null, null, null),
            UpdateType.PollAnswer => (null, null, null),
            UpdateType.MyChatMember => (null, null, null),
            UpdateType.ChatMember => (update.ChatMember!.Chat.Id, null, null),
            UpdateType.ChatJoinRequest => (update.ChatJoinRequest!.UserChatId, null, null),
            UpdateType.MessageReaction => (update.MessageReaction!.Chat.Id, null,
                update.MessageReaction!.MessageId),
            UpdateType.MessageReactionCount => (update.MessageReactionCount!.Chat.Id, null,
                update.MessageReactionCount!.MessageId),
            UpdateType.ChatBoost => (update.ChatBoost!.Chat.Id, null, null),
            UpdateType.RemovedChatBoost => (update.RemovedChatBoost!.Chat.Id, null, null),
            UpdateType.BusinessConnection => (update.BusinessConnection!.UserChatId, null, null),
            UpdateType.BusinessMessage => (update.BusinessMessage!.Chat.Id, update.BusinessMessage!.MessageThreadId,
                update.BusinessMessage!.MessageId),
            UpdateType.EditedBusinessMessage => (update.EditedBusinessMessage!.Chat.Id,
                update.EditedBusinessMessage!.MessageThreadId, update.EditedBusinessMessage!.MessageId),
            UpdateType.DeletedBusinessMessages => (update.DeletedBusinessMessages!.Chat.Id, null, null),
            UpdateType.PurchasedPaidMedia => (null, null, null),
            _ => throw new ArgumentOutOfRangeException(nameof(update)),
        };
        long? userId = update.Type switch
        {
            UpdateType.Unknown => null,
            UpdateType.Message => update.Message!.From?.Id,
            UpdateType.InlineQuery => update.InlineQuery!.From.Id,
            UpdateType.ChosenInlineResult => update.ChosenInlineResult!.From.Id,
            UpdateType.CallbackQuery => update.CallbackQuery!.From.Id,
            UpdateType.EditedMessage => update.EditedMessage!.From?.Id,
            UpdateType.ChannelPost => update.ChannelPost!.From?.Id,
            UpdateType.EditedChannelPost => update.EditedChannelPost!.From?.Id,
            UpdateType.ShippingQuery => update.ShippingQuery!.From.Id,
            UpdateType.PreCheckoutQuery => update.PreCheckoutQuery!.From.Id,
            UpdateType.Poll => null,
            UpdateType.PollAnswer => update.PollAnswer!.User?.Id,
            UpdateType.MyChatMember => update.MyChatMember!.From.Id,
            UpdateType.ChatMember => update.ChatMember!.From.Id,
            UpdateType.ChatJoinRequest => update.ChatJoinRequest!.From.Id,
            UpdateType.MessageReaction => update.MessageReaction!.User?.Id,
            UpdateType.MessageReactionCount => null,
            UpdateType.ChatBoost => null,
            UpdateType.RemovedChatBoost => null,
            UpdateType.BusinessConnection => update.BusinessConnection!.UserChatId,
            UpdateType.BusinessMessage => update.BusinessMessage!.From?.Id,
            UpdateType.EditedBusinessMessage => update.EditedBusinessMessage!.From?.Id,
            UpdateType.DeletedBusinessMessages => null,
            UpdateType.PurchasedPaidMedia => update.PurchasedPaidMedia!.From.Id,
            _ => throw new ArgumentOutOfRangeException(nameof(update)),
        };
        if (BotUsername is null)
        {
            User result = await botClient.GetMe(ct).ConfigureAwait(false);
            BotUsername = result.Username!;
        }

        context.Properties[CULTURE_INFO_PROPS_KEY] =
            lang is null ? GetDefaultCulture(update) : CultureInfo.GetCultureInfoByIetfLanguageTag(lang);
        context.Properties[CHAT_ID_PROPS_KEY] = chatId;
        context.Properties[THREAD_ID_PROPS_KEY] = threadId;
        context.Properties[MESSAGE_ID_PROPS_KEY] = messageId;
        context.Properties[USER_ID_PROPS_KEY] = userId;
        context.Properties[BOT_USERNAME_KEY] = BotUsername;

        await Next(update, context, ct).ConfigureAwait(false);
    }

    protected virtual CultureInfo GetDefaultCulture(in Update update) => CultureInfo.CurrentUICulture;
}