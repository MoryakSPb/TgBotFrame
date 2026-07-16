using System.Reflection;
using System.Runtime.CompilerServices;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.FeatureManagement;
using Microsoft.FeatureManagement.Mvc;
using Telegram.Bot;
using Telegram.Bot.Polling;
using Telegram.Bot.Types.Enums;
using TgBotFrame.Middleware;

namespace TgBotFrame.Services;

/// <summary>
///     Базовый сервис TgBotFrame. Выполняет запуск прослушивания обновлений, создание очереди выполнения ПО промежуточного
///     слоя и инициирует обработку полученных обновлений
/// </summary>
public class BotService(
    ITelegramBotClient botClient,
    ILogger<BotService> logger,
    IServiceScopeFactory scopeFactory,
    FrameMetricsService frameMetricsService) : BackgroundService, IUpdateHandler
{
    public async Task HandleUpdateAsync(ITelegramBotClient _, Update update,
        CancellationToken cancellationToken)
    {
        logger.LogDebug(@"Update {id} received", update.Id);
        await RunMiddleware(update, cancellationToken).ConfigureAwait(false);
    }

    public Task HandleErrorAsync(ITelegramBotClient _, Exception exception, HandleErrorSource source,
        CancellationToken cancellationToken)
    {
        switch (source)
        {
            case HandleErrorSource.PollingError:
                logger.LogError(exception, @"Exception occured during get Telegram updates");
                break;
            case HandleErrorSource.FatalError:
                logger.LogCritical(exception, @"A fatal uncaught exception occured");
                throw exception;
            case HandleErrorSource.HandleUpdateError:
                logger.LogError(exception, @"Exception occured in " + nameof(BotService));
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(source), source, null);
        }

        return Task.CompletedTask;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken) =>
        await botClient.ReceiveAsync(this, new()
        {
            DropPendingUpdates = false,
            AllowedUpdates = Enum.GetValues<UpdateType>(),
        }, stoppingToken).ConfigureAwait(false);

    private async Task RunMiddleware(Update update, CancellationToken cancellationToken = default)
    {
        frameMetricsService.IncUpdatesHandled(update.Type);
        AsyncServiceScope scope = scopeFactory.CreateAsyncScope();
        await using ConfiguredAsyncDisposable _ = scope.ConfigureAwait(false);

        IVariantFeatureManager? featureManager = scope.ServiceProvider.GetService<IVariantFeatureManager>();
        FrameMiddleware[] middlewares;
        if (featureManager is null)
        {
            middlewares = scope.ServiceProvider.GetServices<FrameMiddleware>().ToArray();
        }
        else
        {
            List<FrameMiddleware> frameMiddlewares = [];
            foreach (FrameMiddleware frameMiddleware in scope.ServiceProvider.GetServices<FrameMiddleware>())
            {
                if (await IsFeatureEnabled(featureManager, frameMiddleware.GetType()))
                {
                    frameMiddlewares.Add(frameMiddleware);
                }
            }

            middlewares = frameMiddlewares.ToArray();
        }

        if (middlewares.Length == 0)
        {
            logger.LogWarning(@"There is no registered middlewares, skip update processing");
            return;
        }

        for (int i = 0; i < middlewares.Length - 1; i++)
        {
            middlewares[i].Next = middlewares[i + 1].InvokeAsync;
        }

        middlewares[^1].Next = FrameMiddleware.Empty;
        FrameMiddleware firstMiddleware = middlewares[0];

        using FrameContext context = new();
        await firstMiddleware.InvokeAsync(update, context, cancellationToken).ConfigureAwait(false);
    }

    private static async ValueTask<bool> IsFeatureEnabled(IVariantFeatureManager featureManager, Type type)
    {
        if (type.GetCustomAttribute<FeatureGateAttribute>() is not { } attribute)
        {
            return true;
        }

        return await IsFeatureEnabled(featureManager, attribute);
    }

    private static async ValueTask<bool> IsFeatureEnabled(IVariantFeatureManager? featureManager,
        FeatureGateAttribute? attribute)
    {
        if (featureManager is null) return true;
        switch (attribute?.RequirementType)
        {
            case RequirementType.Any:
                foreach (string? feature in attribute.Features)
                {
                    bool result = await featureManager.IsEnabledAsync(feature);
                    if (result)
                    {
                        return true;
                    }
                }

                return false;
            case RequirementType.All:
                foreach (string? feature in attribute.Features)
                {
                    bool result = await featureManager.IsEnabledAsync(feature);
                    if (!result)
                    {
                        return false;
                    }
                }

                return true;
            case null:
                return true;
            default:
                throw new ArgumentOutOfRangeException();
        }
    }

    public static async Task<bool> IsFeatureEnabled(IVariantFeatureManager? featureManager, MethodInfo method) =>
        await IsFeatureEnabled(featureManager, method.DeclaringType?.GetCustomAttribute<FeatureGateAttribute>())
        && await IsFeatureEnabled(featureManager, method.GetCustomAttribute<FeatureGateAttribute>());
}