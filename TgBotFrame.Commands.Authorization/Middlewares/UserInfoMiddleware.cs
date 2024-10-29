using TgBotFrame.Commands.Authorization.Interfaces;
using TgBotFrame.Middleware;

namespace TgBotFrame.Commands.Authorization.Middlewares;

public class UserInfoMiddleware(IAuthorizationData data) : FrameMiddleware
{
    public const string USERNAME_PROPS_KEY = nameof(User.Username);
    public const string FIRSTNAME_PROPS_KEY = nameof(User.FirstName);
    public const string LASTNAME_PROPS_KEY = nameof(User.LastName);

    public override async Task InvokeAsync(Update update, FrameContext context, CancellationToken ct = default)
    {
        long? userId = context.GetUserId();
        if (userId is null)
        {
            await Next(update, context, ct).ConfigureAwait(false);
            return;
        }

        string? userName = update.Type switch
        {
            UpdateType.Unknown => null,
            UpdateType.Message => update.Message!.From?.Username,
            UpdateType.InlineQuery => update.InlineQuery!.From.Username,
            UpdateType.ChosenInlineResult => update.ChosenInlineResult!.From.Username,
            UpdateType.CallbackQuery => update.CallbackQuery!.From.Username,
            UpdateType.EditedMessage => update.EditedMessage!.From?.Username,
            UpdateType.ChannelPost => update.ChannelPost!.From?.Username,
            UpdateType.EditedChannelPost => update.EditedChannelPost!.From?.Username,
            UpdateType.ShippingQuery => update.ShippingQuery!.From.Username,
            UpdateType.PreCheckoutQuery => update.PreCheckoutQuery!.From.Username,
            UpdateType.Poll => null,
            UpdateType.PollAnswer => update.PollAnswer!.User?.Username,
            UpdateType.MyChatMember => update.MyChatMember!.From.Username,
            UpdateType.ChatMember => update.ChatMember!.From.Username,
            UpdateType.ChatJoinRequest => update.ChatJoinRequest!.From.Username,
            UpdateType.MessageReaction => update.MessageReaction!.User?.Username,
            UpdateType.MessageReactionCount => null,
            UpdateType.ChatBoost => null,
            UpdateType.RemovedChatBoost => null,
            UpdateType.BusinessConnection => update.BusinessConnection!.User.Username,
            UpdateType.BusinessMessage => update.BusinessMessage!.From?.Username,
            UpdateType.EditedBusinessMessage => update.EditedBusinessMessage!.From?.Username,
            UpdateType.DeletedBusinessMessages => null,
            UpdateType.PurchasedPaidMedia => update.PurchasedPaidMedia!.From.Username,
            _ => throw new ArgumentOutOfRangeException(nameof(update)),
        };
        string? firstName = update.Type switch
        {
            UpdateType.Unknown => null,
            UpdateType.Message => update.Message!.From?.FirstName,
            UpdateType.InlineQuery => update.InlineQuery!.From.FirstName,
            UpdateType.ChosenInlineResult => update.ChosenInlineResult!.From.FirstName,
            UpdateType.CallbackQuery => update.CallbackQuery!.From.FirstName,
            UpdateType.EditedMessage => update.EditedMessage!.From?.FirstName,
            UpdateType.ChannelPost => update.ChannelPost!.From?.FirstName,
            UpdateType.EditedChannelPost => update.EditedChannelPost!.From?.FirstName,
            UpdateType.ShippingQuery => update.ShippingQuery!.From.FirstName,
            UpdateType.PreCheckoutQuery => update.PreCheckoutQuery!.From.FirstName,
            UpdateType.Poll => null,
            UpdateType.PollAnswer => update.PollAnswer!.User?.FirstName,
            UpdateType.MyChatMember => update.MyChatMember!.From.FirstName,
            UpdateType.ChatMember => update.ChatMember!.From.FirstName,
            UpdateType.ChatJoinRequest => update.ChatJoinRequest!.From.FirstName,
            UpdateType.MessageReaction => update.MessageReaction!.User?.FirstName,
            UpdateType.MessageReactionCount => null,
            UpdateType.ChatBoost => null,
            UpdateType.RemovedChatBoost => null,
            UpdateType.BusinessConnection => update.BusinessConnection!.User.FirstName,
            UpdateType.BusinessMessage => update.BusinessMessage!.From?.FirstName,
            UpdateType.EditedBusinessMessage => update.EditedBusinessMessage!.From?.FirstName,
            UpdateType.DeletedBusinessMessages => null,
            UpdateType.PurchasedPaidMedia => update.PurchasedPaidMedia!.From.FirstName,
            _ => throw new ArgumentOutOfRangeException(nameof(update)),
        };
        string? lastName = update.Type switch
        {
            UpdateType.Unknown => null,
            UpdateType.Message => update.Message!.From?.LastName,
            UpdateType.InlineQuery => update.InlineQuery!.From.LastName,
            UpdateType.ChosenInlineResult => update.ChosenInlineResult!.From.LastName,
            UpdateType.CallbackQuery => update.CallbackQuery!.From.LastName,
            UpdateType.EditedMessage => update.EditedMessage!.From?.LastName,
            UpdateType.ChannelPost => update.ChannelPost!.From?.LastName,
            UpdateType.EditedChannelPost => update.EditedChannelPost!.From?.LastName,
            UpdateType.ShippingQuery => update.ShippingQuery!.From.LastName,
            UpdateType.PreCheckoutQuery => update.PreCheckoutQuery!.From.LastName,
            UpdateType.Poll => null,
            UpdateType.PollAnswer => update.PollAnswer!.User?.LastName,
            UpdateType.MyChatMember => update.MyChatMember!.From.LastName,
            UpdateType.ChatMember => update.ChatMember!.From.LastName,
            UpdateType.ChatJoinRequest => update.ChatJoinRequest!.From.LastName,
            UpdateType.MessageReaction => update.MessageReaction!.User?.LastName,
            UpdateType.MessageReactionCount => null,
            UpdateType.ChatBoost => null,
            UpdateType.RemovedChatBoost => null,
            UpdateType.BusinessConnection => update.BusinessConnection!.User.LastName,
            UpdateType.BusinessMessage => update.BusinessMessage!.From?.LastName,
            UpdateType.EditedBusinessMessage => update.EditedBusinessMessage!.From?.LastName,
            UpdateType.DeletedBusinessMessages => null,
            UpdateType.PurchasedPaidMedia => update.PurchasedPaidMedia!.From.LastName,
            _ => throw new ArgumentOutOfRangeException(nameof(update)),
        };

        if (firstName is not null)
        {
            DbUser? entity = await data.Users.AsTracking().FirstOrDefaultAsync(x => x.Id == userId, ct)
                .ConfigureAwait(false);
            if (entity is null)
            {
                entity = new()
                {
                    Id = userId.Value,
                    UserName = userName,
                    FirstName = firstName,
                    LastName = lastName,
                };
                data.Users.Add(entity);
            }
            else
            {
                if (entity.UserName != userName) entity.UserName = userName;
                if (entity.FirstName != firstName) entity.FirstName = firstName;
                if (entity.LastName != lastName) entity.LastName = lastName;
            }

            await data.SaveChangesAsync(ct).ConfigureAwait(false);

            context.Properties[USERNAME_PROPS_KEY] = entity.UserName;
            context.Properties[FIRSTNAME_PROPS_KEY] = entity.FirstName;
            context.Properties[LASTNAME_PROPS_KEY] = entity.LastName;
        }

        await Next(update, context, ct).ConfigureAwait(false);
    }
}