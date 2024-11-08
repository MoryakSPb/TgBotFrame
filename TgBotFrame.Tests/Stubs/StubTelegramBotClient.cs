using Telegram.Bot;
using Telegram.Bot.Args;
using Telegram.Bot.Exceptions;
using Telegram.Bot.Requests.Abstractions;

namespace TgBotFrame.Tests.Stubs;

public class StubTelegramBotClient : ITelegramBotClient
{
    public Task<TResponse> SendRequest<TResponse>(IRequest<TResponse> request,
        CancellationToken cancellationToken = default) => throw new NotImplementedException();

    public Task<TResponse> MakeRequest<TResponse>(IRequest<TResponse> request,
        CancellationToken cancellationToken = default) => throw new NotImplementedException();

    public Task<TResponse> MakeRequestAsync<TResponse>(IRequest<TResponse> request,
        CancellationToken cancellationToken = default) => throw new NotImplementedException();

    public Task<bool> TestApi(CancellationToken cancellationToken = default) => Task.FromResult(true);

    public Task DownloadFile(string filePath, Stream destination, CancellationToken cancellationToken = default) =>
        throw new NotImplementedException();

    public bool LocalBotServer => true;
    public long BotId => -1;
    public TimeSpan Timeout { get; set; } = TimeSpan.MaxValue;
    public IExceptionParser ExceptionsParser { get; set; } = new DefaultExceptionParser();
    public event AsyncEventHandler<ApiRequestEventArgs>? OnMakingApiRequest;
    public event AsyncEventHandler<ApiResponseEventArgs>? OnApiResponseReceived;
}