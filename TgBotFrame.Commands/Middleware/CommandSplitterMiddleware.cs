using System.Buffers;
using TgBotFrame.Commands.Extensions;

namespace TgBotFrame.Commands.Middleware;

public class CommandSplitterMiddleware : FrameMiddleware
{
    public const string COMMAND_NAME_KEY = "CommandName";
    public const string COMMAND_ARGS_KEY = "CommandArguments";

    public override async Task InvokeAsync(Update update, FrameContext context, CancellationToken ct = default)
    {
        string? text = null;

        if (update.CallbackQuery is not null && update.CallbackQuery.Data?[0] == '/')
            text = update.CallbackQuery.Data;
        else if (update.Message?.Text is not null && update.Message.Text[0] == '/') text = update.Message.Text;

        if (text is not null)
        {
            string commandKey = SplitCommand(text, out IReadOnlyCollection<string> args);
            int atIndex = commandKey.IndexOf('@');
            if (atIndex >= 0)
            {
                if (commandKey[(atIndex + 1)..] != context.GetBotUsername())
                {
                    await Next(update, context, ct).ConfigureAwait(false);
                    return;
                }

                commandKey = commandKey[..atIndex];
            }

            context.Properties[COMMAND_NAME_KEY] = commandKey;
            context.Properties[COMMAND_ARGS_KEY] = args;
        }

        await Next(update, context, ct).ConfigureAwait(false);
    }

    private static string SplitCommand(string input, out IReadOnlyCollection<string> args)
    {
        if (input.Length < 2 || input[0] != '/')
        {
            args = [];
            return string.Empty;
        }

        List<string> argsList = [];
        args = argsList;
        char[]? array = null;
        if (input.Length > 64) array = ArrayPool<char>.Shared.Rent(input.Length);
        Span<char> chars = array ?? stackalloc char[input.Length];
        string command = string.Empty;
        try
        {
            bool escapeMode = false;
            bool escapeOnce = false;
            bool commandMode = true;
            int count = 0;
            for (int i = 1; i < input.Length; i++)
            {
                char c = input[i];
                switch (c)
                {
                    case '\\':
                        escapeOnce = true;
                        continue;
                    case '\"' when !escapeOnce:
                        escapeMode = !escapeMode;
                        continue;
                    case ' ' when !(escapeMode || escapeOnce):
                        if (count == 0) continue;
                        if (commandMode)
                        {
                            command = new(chars[..count]);
                            commandMode = false;
                        }
                        else
                        {
                            argsList.Add(new(chars[..count]));
                        }

                        count = 0;
                        continue;
                    default:
                        chars[count] = c;
                        count++;
                        escapeOnce = false;
                        continue;
                }
            }

            count -= escapeOnce ? 1 : 0;
            if (commandMode)
            {
                return new(chars[..count]);
            }
            else
            {
                argsList.Add(new(chars[..count]));
                return command;
            }
        }
        finally
        {
            if (array is not null) ArrayPool<char>.Shared.Return(array);
        }
    }
}