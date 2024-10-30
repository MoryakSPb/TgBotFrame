using Telegram.Bot;
using Telegram.Bot.Args;
using Telegram.Bot.Exceptions;
using Telegram.Bot.Requests.Abstractions;

namespace TgBotFrame.Tests.Stubs;

public class StubTelegramBotClient : ITelegramBotClient
{
    public async Task<TResponse> MakeRequestAsync<TResponse>(IRequest<TResponse> request, CancellationToken cancellationToken = default) => throw new NotImplementedException();

    public Task<bool> TestApiAsync(CancellationToken cancellationToken = default) => Task.FromResult(true);

    public async Task DownloadFileAsync(string filePath, Stream destination,
        CancellationToken cancellationToken = new CancellationToken()) =>
        throw new NotImplementedException();

    public bool LocalBotServer => true;
    public long BotId => -1;
    public TimeSpan Timeout { get; set; } = TimeSpan.MaxValue;
    public IExceptionParser ExceptionsParser { get; set; }
    public event AsyncEventHandler<ApiRequestEventArgs>? OnMakingApiRequest;
    public event AsyncEventHandler<ApiResponseEventArgs>? OnApiResponseReceived;
}