using Microsoft.Extensions.Logging;
using Telegram.Bot;
using Telegram.Bot.Types;
using TgBotFrame.Commands.Services;

namespace TgBotFrame.Commands.Help.Services;

public class HelpCommandService(ITelegramBotClient botClient, CommandExplorerService commandExplorer)
{
    public async Task SetMyCommands()
    {
        IEnumerable<BotCommand> commands = commandExplorer.Commands.Keys
            .Select(it => new BotCommand(it.ToLower(), it));
        BotCommandScopeAllPrivateChats allPrivateChats = new ();
        //BotCommandScopeAllChatAdministrators
        //BotCommandScopeAllChatAdministrators
        await botClient.SetMyCommands(commands,allPrivateChats);
    }
}