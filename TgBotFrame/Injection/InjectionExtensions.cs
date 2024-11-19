using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Hosting;
using Polly;
using Polly.Contrib.WaitAndRetry;
using Polly.Extensions.Http;
using Telegram.Bot;
using TgBotFrame.Services;
using TgBotFrame.Utility;

namespace TgBotFrame.Injection;

public static class InjectionExtensions
{
    /// <summary>
    ///     Добавляет базовые сервисы для работы других компонентов TgBotFrame
    /// </summary>
    /// <param name="serviceCollection">Коллекция сервисов</param>
    /// <returns>Экземпляр из аргумента serviceCollection</returns>
    /// <exception cref="KeyNotFoundException">ITelegramBotClient не зарегистрирован</exception>
    public static IServiceCollection AddTgBotFrameCore(this IServiceCollection serviceCollection)
    {
        if (serviceCollection.All(x =>
                x.ServiceType != typeof(ITelegramBotClient) && x.Lifetime == ServiceLifetime.Singleton))
        {
            throw new KeyNotFoundException(@"Singleton ITelegramBotClient service not found");
        }

        serviceCollection.AddMetrics();
        serviceCollection.TryAddSingleton<FrameMetricsService>();

        serviceCollection.TryAddSingleton<BotService>();
        serviceCollection.TryAddEnumerable(new ServiceDescriptor(typeof(IHostedService), typeof(BotService),
            ServiceLifetime.Singleton));

        return serviceCollection;
    }

    /// <summary>
    ///     Добавляет HttpClient для TgBotFrame, поддерживающий повторение и лимит запросов
    /// </summary>
    /// <param name="serviceCollection">Коллекция сервисов</param>
    /// <returns>Экземпляр из аргумента serviceCollection</returns>
    public static IServiceCollection AddTelegramHttpClient(this IServiceCollection serviceCollection)
    {
        serviceCollection.AddScoped<TelegramRateLimitedHandler>();
        serviceCollection.AddHttpClient<ITelegramBotClient, TelegramBotClient>()
            .SetHandlerLifetime(TimeSpan.FromMinutes(3))
            .AddPolicyHandler(PolicySelector)
            .AddHttpMessageHandler<TelegramRateLimitedHandler>();

        return serviceCollection;

        static IAsyncPolicy<HttpResponseMessage> PolicySelector(HttpRequestMessage request)
        {
            IEnumerable<TimeSpan> delay = Backoff.DecorrelatedJitterBackoffV2(TimeSpan.FromSeconds(1), 5);

            return HttpPolicyExtensions.HandleTransientHttpError()
                .WaitAndRetryAsync(delay);
        }
    }
}