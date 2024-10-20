﻿using System.Collections.Frozen;
using System.Reflection;
using System.Resources;
using System.Text;
using Telegram.Bot;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;
using TgBotFrame.Commands.Attributes;
using TgBotFrame.Commands.Extensions;
using TgBotFrame.Commands.Help.Extensions;
using TgBotFrame.Commands.Help.Properties;
using TgBotFrame.Commands.Services;

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

        await botClient.SendTextMessageAsync(
            Context.GetChatId()!,
            Resources.ResourceManager.GetString(nameof(HelpCommandController_Help_Title),
                Context.GetCultureInfo())!,
            Context.GetThreadId(),
            ParseMode.None,
            [],
            replyMarkup: keyboard
        ).ConfigureAwait(false);
    }

    [Command(nameof(HelpSyntax))]
    public async Task HelpSyntax()
    {
        await botClient.SendTextMessageAsync(
            Context.GetChatId()!,
            Resources.ResourceManager.GetString(nameof(HelpCommandController_HelpSyntax_Title),
                Context.GetCultureInfo())!,
            Context.GetThreadId(),
            ParseMode.None,
            []
        ).ConfigureAwait(false);
    }

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
            .Select(x => (GetResourceManager(x?.Assembly),
                x?.GetCustomAttribute<CommandControllerAttribute>()?.CategoryKey))
            .Distinct()
            .Select(x =>
            {
                string displayName;
                if (string.IsNullOrEmpty(x.CategoryKey))
                    displayName = Resources.ResourceManager.GetString(
                        nameof(HelpCommandController_HelpList_WithoutCategory),
                        Context.GetCultureInfo())!;
                else if (x.Item1 is not null)
                    displayName = x.Item1.GetString(ResourcesExtensions.CATEGORY_NAME_PREFIX + x.CategoryKey,
                        Context.GetCultureInfo()) ?? throw new KeyNotFoundException();
                else throw new InvalidOperationException();

                return (x.Item1, x.CategoryKey, displayName);
            }).OrderBy(x => x.displayName, StringComparer.Create(Context.GetCultureInfo(), true)).Select(x => new[]
            {
                InlineKeyboardButton.WithCallbackData(
                    x.displayName,
                    $@"/{nameof(HelpCategory)} {x.CategoryKey}"),
            });


        await botClient.SendTextMessageAsync(
            Context.GetChatId()!,
            Resources.ResourceManager.GetString(nameof(HelpCommandController_HelpList_Description),
                Context.GetCultureInfo())!,
            Context.GetThreadId(),
            ParseMode.None,
            [],
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
            await botClient.SendTextMessageAsync(
                Context.GetChatId()!,
                Resources.ResourceManager.GetString(nameof(HelpCommandController_HelpCategory_NotFound),
                    Context.GetCultureInfo())!,
                Context.GetThreadId(),
                ParseMode.None,
                [],
                replyMarkup: null).ConfigureAwait(false);
            return;
        }

        IEnumerable<InlineKeyboardButton[]> buttons = commands.commands.Select(x => new[]
            { InlineKeyboardButton.WithCallbackData(x, $@"/{nameof(HelpCommand)} {x}") });

        string text = (category.Length == 0
            ? Resources.ResourceManager.GetString(
                nameof(HelpCommandController_HelpCategory_NoCategory),
                Context.GetCultureInfo())
            : GetResourceManager(commands.assembly)?.GetString(
                ResourcesExtensions.CATEGORY_DESCRIPTION_PREFIX + category,
                Context.GetCultureInfo())) ?? throw new KeyNotFoundException();

        await botClient.SendTextMessageAsync(
            Context.GetChatId()!,
            text,
            Context.GetThreadId(),
            ParseMode.None,
            [],
            replyMarkup: new InlineKeyboardMarkup(buttons)).ConfigureAwait(false);
    }

    [Command(nameof(HelpCommand))]
    public async Task HelpCommand(string command)
    {
        if (!commandExplorer.Commands.TryGetValue(command, out FrozenDictionary<MethodInfo, ParameterInfo[]>? methods)
            || methods.Count == 0)
        {
            await botClient.SendTextMessageAsync(
                Context.GetChatId()!,
                Resources.ResourceManager.GetString(nameof(HelpCommandController_HelpCommand_NotFound),
                    Context.GetCultureInfo())!,
                Context.GetThreadId(),
                ParseMode.None,
                [],
                replyMarkup: null).ConfigureAwait(false);
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

        text.Append(resourceManager?.GetString(ResourcesExtensions.COMMAND_DESCRIPTION_PREFIX + command,
            Context.GetCultureInfo()));

        await botClient.SendTextMessageAsync(
            Context.GetChatId()!,
            text.ToString(),
            Context.GetThreadId(),
            ParseMode.None,
            [],
            replyMarkup: null).ConfigureAwait(false);
    }

    private static ResourceManager? GetResourceManager(Assembly? source)
    {
        Type[]? types = source?.GetExportedTypes();
        Type? resourcesType = types?.FirstOrDefault(x => x.Name == nameof(Resources));
        PropertyInfo? property = resourcesType?.GetProperty(
            "ResourceManager",
            BindingFlags.GetProperty | BindingFlags.Public | BindingFlags.Static);
        return (ResourceManager?)property?.GetMethod?.Invoke(null, []);
    }
}