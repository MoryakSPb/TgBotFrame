using System.Collections.Frozen;
using System.Globalization;
using System.Reflection;
using System.Runtime.CompilerServices;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Telegram.Bot;
using Telegram.Bot.Types.Enums;
using TgBotFrame.Commands.Extensions;
using TgBotFrame.Commands.Properties;
using TgBotFrame.Commands.Services;

namespace TgBotFrame.Commands.Middleware;

public sealed class CommandRouterMiddleware(
    CommandExplorerService commandExplorerService,
    IServiceScopeFactory scopeFactory,
    ITelegramBotClient botClient,
    ILogger<CommandRouterMiddleware> logger) : FrameMiddleware
{
    public const string COMMAND_CONTROLLER_KEY = "CommandController";
    public const string COMMAND_METHOD_KEY = "CommandMethod";
    public const string COMMAND_ARGS_KEY = "CommandArguments";
    private string CommandKey { get; set; } = string.Empty;
    private CultureInfo Culture { get; set; } = CultureInfo.CurrentUICulture;
    private string ExceptedName { get; set; } = string.Empty;
    private IList<string> CommandArgumentsRaw { get; set; } = [];

    public override async Task InvokeAsync(Update update, FrameContext context, CancellationToken ct = default)
    {
        CommandKey = context.GetCommandName() ?? string.Empty;
        if (CommandKey.Length == 0)
        {
            await Next(update, context, ct).ConfigureAwait(false);
            return;
        }

        Culture = context.GetCultureInfo();
        ExceptedName = context.GetBotUsername();
        CommandArgumentsRaw = context.GetCommandArgsRaw();

        if (!commandExplorerService.Commands.TryGetValue(CommandKey,
                out FrozenDictionary<MethodInfo, ParameterInfo[]>? allMethods) || allMethods.Count == 0)
        {
            await SendCommandNotFound(update, context, ct).ConfigureAwait(false);
            await Next(update, context, ct).ConfigureAwait(false);
            return;
        }

        (MethodInfo? methodInfo, int invalidArgIndex) = GetMethod(allMethods, update.Message!, out object?[] args);
        if (methodInfo is null)
        {
            switch (invalidArgIndex)
            {
                case int.MaxValue:
                    await SendAmbiguousCall(update, context, ct).ConfigureAwait(false);
                    break;
                case >= 0:
                    await SendInvalidArgFormat(update, context, invalidArgIndex, ct).ConfigureAwait(false);
                    break;
                default:
                    await SendInvalidArgsCount(update, context, ct).ConfigureAwait(false);
                    break;
            }

            await Next(update, context, ct).ConfigureAwait(false);
            return;
        }

        AsyncServiceScope scope = scopeFactory.CreateAsyncScope();
        await using ConfiguredAsyncDisposable _ = scope.ConfigureAwait(false);

        object? controller = methodInfo.DeclaringType is null
            ? null
            : scope.ServiceProvider.GetRequiredService(methodInfo.DeclaringType);

        context.Properties[COMMAND_CONTROLLER_KEY] = controller;
        context.Properties[COMMAND_METHOD_KEY] = methodInfo;
        context.Properties[COMMAND_ARGS_KEY] = args;

        logger.LogInformation("User {userId} enter command {commandName}({commandArgs})",
            context.GetUserId(),
            context.GetCommandName(),
            string.Join(", ", args.Select(y => y?.GetType()?.Name)));

        await Next(update, context, ct).ConfigureAwait(false);
    }


    private async Task SendCommandNotFound(Update update, FrameContext context, CancellationToken ct = default)
    {
        logger.LogInformation("User <{userId}> enter unknown command {commandName}",
            context.GetUserId(),
            context.GetCommandName());
        await botClient.SendMessage(update.Message!.Chat,
            Resources.ResourceManager.GetString(@"CommandNotFoundMessage", Culture)!
            + Environment.NewLine
            + string.Format(Resources.ResourceManager.GetString(@"UseHelpMessage", Culture)!,
                ExceptedName),
            replyParameters: new()
            {
                MessageId = update.Message.MessageId,
            },
            cancellationToken: ct).ConfigureAwait(false);
    }

    private async Task SendInvalidArgsCount(Update update, FrameContext context, CancellationToken ct = default)
    {
        logger.LogInformation("User <{userId}> enter command \"{commandName}\" with invalid args count: {args}",
            context.GetUserId(),
            context.GetCommandName(),
            Environment.NewLine + string.Join(Environment.NewLine, context.GetCommandArgsRaw()));
        await botClient.SendMessage(update.Message!.Chat,
            Resources.ResourceManager.GetString(@"InvalidArgsCountMessage", Culture)
            + Environment.NewLine
            + string.Format(Resources.ResourceManager.GetString(@"UseHelpMessage", Culture)!,
                ExceptedName), replyParameters: new()
                {
                    MessageId = update.Message.MessageId,
                },
            cancellationToken: ct).ConfigureAwait(false);
    }

    private async Task SendAmbiguousCall(Update update, FrameContext context, CancellationToken ct = default)
    {
        logger.LogWarning(@"User <{userId}> enter ambiguous command ""{commandName}""",
            context.GetUserId(),
            context.GetCommandName());
        await botClient.SendMessage(update.Message!.Chat,
            string.Format(Resources.ResourceManager.GetString(@"AmbiguousCall", Culture)!, CommandKey)
            + Environment.NewLine
            + Resources.ResourceManager.GetString(@"ContactWithAdminMessage", Culture)!,
            replyParameters: new()
            {
                MessageId = update.Message.MessageId,
            },
            cancellationToken: ct).ConfigureAwait(false);
    }

    private async Task SendInvalidArgFormat(Update update, FrameContext context, int invalidArgIndex,
        CancellationToken ct)
    {
        logger.LogWarning(
            @"User <{userId}> enter command ""{commandName}"" with invalid format in arg #{argNum:D}: ""{arg}""",
            context.GetUserId(),
            context.GetCommandName(),
            invalidArgIndex,
            context.GetCommandArgsRaw()[invalidArgIndex]);
        await botClient.SendMessage(update.Message!.Chat,
            string.Format(
                Resources.ResourceManager.GetString(@"InvalidArgFormatMessage",
                    Culture)!, context.GetCommandArgsRaw()[invalidArgIndex])
            + Environment.NewLine
            + string.Format(
                Resources.ResourceManager.GetString(@"UseHelpMessage", Culture)!,
                ExceptedName),
            replyParameters: new()
            {
                MessageId = update.Message.MessageId,
            }, cancellationToken: ct).ConfigureAwait(false);
    }

    private (MethodInfo?, int invalidArgIndex) GetMethod(
        in FrozenDictionary<MethodInfo, ParameterInfo[]> allMethods, in Message message, out object?[] args)
    {
        args = [];
        KeyValuePair<MethodInfo, ParameterInfo[]> method;
        if (allMethods.Count == 1 && (method = allMethods.First()).Value.Length == 0)
        {
            return (method.Key, -1);
        }

        Dictionary<int, MethodCandidate> candidates = allMethods
            .Where(x => x.Value.Length == CommandArgumentsRaw.Count)
            .Select(x => new MethodCandidate(x.Key, x.Value))
            .Zip(Enumerable.Range(0, allMethods.Count))
            .ToDictionary(x => x.Second, x => x.First);
        if (candidates.Count == 0)
        {
            return (default, -1);
        }

        List<int> candidatesToRemove = new(candidates.Count);

        int mentionCounter = 0;
        for (int i = 0; i < CommandArgumentsRaw.Count; i++)
        {
            string rawArg = CommandArgumentsRaw[i];

            foreach (KeyValuePair<int, MethodCandidate> pair in candidates)
            {
                ParameterInfo parameter = pair.Value.Parameters[i];
                if (parameter.ParameterType == typeof(string))
                {
                    pair.Value.Args[i] = rawArg;
                }
                else if (parameter.ParameterType == typeof(User))
                {
                    throw new NotSupportedException();
                    MessageEntity? mention = message.Entities
                        ?.Where(x => x.Type == MessageEntityType.Mention)
                        .Skip(mentionCounter).FirstOrDefault();
                    if (mention is null)
                    {
                        if (message.ReplyToMessage?.From is not null && mentionCounter == 0)
                        {
                            pair.Value.Args[i] = message.ReplyToMessage.From;
                        }
                        else
                        {
                            candidatesToRemove.Add(pair.Key);
                            continue;
                        }
                    }
                    else
                    {
                        if (mention.User is not null)
                        {
                            pair.Value.Args[i] = mention.User;
                        }
                    }

                    mentionCounter++;
                }
                else if (parameter.ParameterType.GetInterfaces()
                         .Contains(typeof(IParsable<>).MakeGenericType(parameter.ParameterType)))
                {
                    MethodInfo parseMethod = parameter.ParameterType.GetMethod(
                                                 nameof(int.TryParse),
                                                 BindingFlags.Static | BindingFlags.Public,
                                                 [
                                                     typeof(string),
                                                     typeof(IFormatProvider),
                                                     parameter.ParameterType.MakeByRefType(),
                                                 ])
                                             ?? throw new InvalidOperationException();
                    object?[] parseArgs = [CommandArgumentsRaw[i], Culture, null];
                    bool result = (bool)parseMethod.Invoke(null, parseArgs)!;
                    if (!result)
                    {
                        parseArgs[1] = CultureInfo.InvariantCulture;
                        result = (bool)parseMethod.Invoke(null, parseArgs)!;
                    }

                    if (result)
                    {
                        pair.Value.Args[i] = parseArgs[2]!;
                    }
                    else
                    {
                        candidatesToRemove.Add(pair.Key);
                    }
                }
                else
                {
                    throw new NotSupportedException();
                }
            }

            if (candidatesToRemove.Where(candidates.Remove).Any(_ => candidates.Count == 0))
            {
                return (null, i);
            }
        }

        if (candidates.Count > 1)
        {
            return (null, int.MaxValue);
        }

        MethodCandidate resultStruct = candidates.Values.First();
        args = resultStruct.Args;
        return (resultStruct.Method, -1);
    }

    private readonly struct MethodCandidate(in MethodInfo method, in ParameterInfo[] parameters)
    {
        public MethodInfo Method { get; } = method;
        public ParameterInfo[] Parameters { get; } = parameters;
        public object[] Args { get; } = new object[parameters.Length];
    }
}