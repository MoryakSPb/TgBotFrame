using System.Reflection;
using System.Resources;
using System.Text;
using Microsoft.FeatureManagement;
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
public class HelpCommandController(
    ITelegramBotClient botClient,
    CommandExplorerService commandExplorer,
    IVariantFeatureManager? featureManager = null)
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


        IAsyncEnumerable<InlineKeyboardButton[]> buttons = commandExplorer.GetCommands(featureManager)
            .SelectMany(x => x.Value.Select(y => y.Key))
            .Select(x => x.DeclaringType)
            .Select(x => x?.GetCustomAttribute<CommandControllerAttribute>()?.CategoryKey)
            .Distinct()
            .Select(categoryName =>
            {
                string displayName;
                if (string.IsNullOrEmpty(categoryName))
                {
                    displayName = Resources.ResourceManager.GetString(
                        nameof(HelpCommandController_HelpList_WithoutCategory),
                        Context.GetCultureInfo())!;
                }
                else
                {
                    string? str = commandExplorer.GetAssembliesForCategory(categoryName)
                        .Select(y =>
                            GetResourceManager(y)?.GetString(CATEGORY_NAME_PREFIX + categoryName,
                                Context.GetCultureInfo()))
                        .FirstOrDefault(y => !string.IsNullOrEmpty(y));
                    displayName = str ?? categoryName;
                }

                return (x: categoryName, displayName);
            }).OrderBy(x => x.displayName, StringComparer.Create(Context.GetCultureInfo(), true)).Select(x => new[]
            {
                InlineKeyboardButton.WithCallbackData(
                    x.displayName,
                    $@"/{nameof(HelpCategory)} {x.x}"),
            });


        await botClient.SendMessage(
            Context.GetUserId()!,
            Resources.ResourceManager.GetString(nameof(HelpCommandController_HelpList_Description),
                Context.GetCultureInfo())!,
            replyMarkup: new InlineKeyboardMarkup(await buttons.ToArrayAsync())
        ).ConfigureAwait(false);
    }

    [Command(nameof(HelpCategory))]
    public Task HelpCategory() => HelpCategory(string.Empty);

    [Command(nameof(HelpCategory))]
    public async Task HelpCategory(string category)
    {
        string[] commands = await commandExplorer.GetCategoryCommandsNames(featureManager, category).ToArrayAsync();
        if (commands.Length == 0)
        {
            await botClient.SendMessage(
                Context.GetUserId()!,
                Resources.ResourceManager.GetString(nameof(HelpCommandController_HelpCategory_NotFound),
                    Context.GetCultureInfo())!).ConfigureAwait(false);
            return;
        }

        IEnumerable<InlineKeyboardButton[]> buttons = commands.Select(x => new[]
            { InlineKeyboardButton.WithCallbackData(x, $@"/{nameof(HelpCommand)} {x}") });

        string text;
        if (category.Length == 0)
        {
            text = Resources.ResourceManager.GetString(
                nameof(HelpCommandController_HelpCategory_NoCategory),
                Context.GetCultureInfo()) ?? category;
        }
        else
        {
            text = commandExplorer.GetAssembliesForCategory(category).Select(x => GetResourceManager(x)
                ?.GetString(CATEGORY_DESCRIPTION_PREFIX + category,
                    Context.GetCultureInfo())).FirstOrDefault(x => !string.IsNullOrEmpty(x)) ?? category;
        }

        await botClient.SendMessage(
            Context.GetUserId()!,
            text,
            replyMarkup: new InlineKeyboardMarkup(buttons)).ConfigureAwait(false);
    }

    [Command(nameof(HelpCommand))]
    public async Task HelpCommand(string command)
    {
        Dictionary<MethodInfo, ParameterInfo[]> methods =
            await commandExplorer.GetCommand(featureManager, command).ToDictionaryAsync();
        if (methods.Count == 0)
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