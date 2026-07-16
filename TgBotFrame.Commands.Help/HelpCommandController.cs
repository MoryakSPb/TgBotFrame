using System.Collections.Frozen;
using System.Reflection;
using System.Resources;
using System.Text;
using Telegram.Bot;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;
using TgBotFrame.Commands.Attributes;
using TgBotFrame.Commands.Extensions;
using TgBotFrame.Commands.Help.Properties;
using TgBotFrame.Commands.Services;
using static TgBotFrame.Commands.Help.Extensions.ResourcesExtensions;

namespace TgBotFrame.Commands.Help;

[CommandController(nameof(Help))]
public class HelpCommandController(ITelegramBotClient botClient, CommandExplorerService commandExplorer)
    : CommandControllerBase
{
    [Command(nameof(Help))]
    public async Task Help()
    {
        InlineKeyboardMarkup keyboard = new((InlineKeyboardButton[])
        [
            InlineKeyboardButton.WithCallbackData(
                Resources.ResourceManager.GetString(@"Help_Button_List", Context.GetCultureInfo())
                ?? throw new ArgumentOutOfRangeException(),
                @$"/{nameof(HelpList)}"),
            InlineKeyboardButton.WithCallbackData(
                Resources.ResourceManager.GetString(@"Help_Button_Syntax", Context.GetCultureInfo())
                ?? throw new ArgumentOutOfRangeException(),
                @$"/{nameof(HelpSyntax)}"),
        ]);

        await botClient.SendMessage(
            Context.GetUserId()!,
            Resources.ResourceManager.GetString(nameof(HelpCommandController_Help_Title),
                Context.GetCultureInfo())!,
            replyMarkup: keyboard
        ).ConfigureAwait(false);
    }

    [Command(nameof(HelpSyntax))]
    public async Task HelpSyntax() =>
        await botClient.SendMessage(
            Context.GetUserId()!,
            Resources.ResourceManager.GetString(nameof(HelpCommandController_HelpSyntax_Title),
                Context.GetCultureInfo())!,
            ParseMode.MarkdownV2).ConfigureAwait(false);

    [Command(nameof(HelpList))]
    public async Task HelpList()
    {
        StringBuilder text = new(Resources.ResourceManager.GetString(nameof(HelpCommandController_HelpList_Title),
            Context.GetCultureInfo())!);
        text.AppendLine();
        text.AppendLine();

        IEnumerable<InlineKeyboardButton[]> buttons = commandExplorer.Commands.Values
            .SelectMany(x => x.Keys)
            .Select(x => x.DeclaringType)
            .Select(x => (x?.Assembly,
                x?.GetCustomAttribute<CommandControllerAttribute>()?.CategoryKey))
            .GroupBy(x => x.CategoryKey, StringComparer.OrdinalIgnoreCase)
            .Select(x =>
            {
                string displayName;
                if (string.IsNullOrEmpty(x.Key))
                {
                    displayName = Resources.ResourceManager.GetString(
                        nameof(HelpCommandController_HelpList_WithoutCategory),
                        Context.GetCultureInfo())!;
                }
                else
                {
                    displayName = x.Select(y => GetResourceManager(y.Assembly)
                                          ?.GetString(CATEGORY_NAME_PREFIX + x.Key, Context.GetCultureInfo()))
                                      .FirstOrDefault(y => !string.IsNullOrEmpty(y))
                                  ?? x.Key;
                }

                return (displayName, x.Key);
            }).OrderBy(x => x.displayName, StringComparer.Create(Context.GetCultureInfo(), true)).Select(x => new[]
            {
                InlineKeyboardButton.WithCallbackData(
                    x.displayName,
                    $@"/{nameof(HelpCategory)} {x.Key}"),
            });


        await botClient.SendMessage(
            Context.GetUserId()!,
            Resources.ResourceManager.GetString(nameof(HelpCommandController_HelpList_Description),
                Context.GetCultureInfo())!,
            replyMarkup: new InlineKeyboardMarkup(buttons)
        ).ConfigureAwait(false);
    }

    [Command(nameof(HelpCategory))]
    public Task HelpCategory() => HelpCategory(string.Empty);

    [Command(nameof(HelpCategory))]
    public async Task HelpCategory(string category)
    {
        if (!commandExplorer.CategoriesCommandsNames.TryGetValue(category,
                out (Assembly assembly, FrozenSet<string> commands) commands))
        {
            await botClient.SendMessage(
                Context.GetUserId()!,
                Resources.ResourceManager.GetString(nameof(HelpCommandController_HelpCategory_NotFound),
                    Context.GetCultureInfo())!).ConfigureAwait(false);
            return;
        }

        IEnumerable<InlineKeyboardButton[]> buttons = commands.commands.Select(x => new[]
            { InlineKeyboardButton.WithCallbackData(x, $@"/{nameof(HelpCommand)} {x}") });

        string text = (category.Length == 0
            ? Resources.ResourceManager.GetString(
                nameof(HelpCommandController_HelpCategory_NoCategory),
                Context.GetCultureInfo())
            : GetResourceManager(commands.assembly)?.GetString(
                CATEGORY_DESCRIPTION_PREFIX + category,
                Context.GetCultureInfo())) ?? category;

        await botClient.SendMessage(
            Context.GetUserId()!,
            text,
            replyMarkup: new InlineKeyboardMarkup(buttons)).ConfigureAwait(false);
    }

    [Command(nameof(HelpCommand))]
    public async Task HelpCommand(string command)
    {
        if (!commandExplorer.Commands.TryGetValue(command,
                out FrozenDictionary<MethodInfo, ParameterInfo[]>? methods)
            || methods.Count == 0)
        {
            await botClient.SendMessage(
                Context.GetUserId()!,
                Resources.ResourceManager.GetString(nameof(HelpCommandController_HelpCommand_NotFound),
                    Context.GetCultureInfo())!).ConfigureAwait(false);
            return;
        }

        ResourceManager? resourceManager = GetResourceManager(methods.First().Key.DeclaringType?.Assembly);
        StringBuilder text = new(command);
        text.AppendLine();

        text.Append(Resources.ResourceManager.GetString(nameof(HelpCommandController_HelpCommand_Overloads),
            Context.GetCultureInfo())!);
        foreach (KeyValuePair<MethodInfo, ParameterInfo[]> method in methods)
        {
            text.AppendLine();
            text.Append('/');
            text.Append(command);
            foreach (ParameterInfo parameterInfo in method.Value)
            {
                text.Append(' ');
                text.Append(parameterInfo.ParameterType.GetFormatText(Context.GetCultureInfo(), true));
            }
        }

        text.AppendLine();
        text.AppendLine();

        text.Append(resourceManager?.GetString(COMMAND_DESCRIPTION_PREFIX + command,
            Context.GetCultureInfo()));

        await botClient.SendMessage(
            Context.GetUserId()!,
            text.ToString()).ConfigureAwait(false);
    }
}