using System.Text.Json;
using System.Text.Json.Serialization;
using Telegram.Bot;
using Telegram.Bot.Args;
using Telegram.Bot.Exceptions;
using Telegram.Bot.Requests;
using Telegram.Bot.Requests.Abstractions;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace TgBotFrame.Tests.Stubs;

public class StubTelegramBotClient : ITelegramBotClient
{
    public Queue<SendMessageRequest> Messages { get; } = new();
    public Task<TResponse> SendRequest<TResponse>(IRequest<TResponse> request,
        CancellationToken cancellationToken = default)
    {
        switch (request)
        {
            case GetMeRequest:
            {
                User user = new()
                {
                    Id = 0,
                    Username = "TEST",
                    LanguageCode = "en",
                    FirstName = "TEST_NAME",
                    IsBot = true,
                };
                string json = JsonSerializer.Serialize(user);
                return Task.FromResult(JsonSerializer.Deserialize<TResponse>(json)!);
            }
            case SendMessageRequest sendMessageRequest:
            {
                Message message = new()
                {
                    Id = Random.Shared.Next(0),
                    Chat = new()
                    {
                        Id = sendMessageRequest.ChatId.Identifier.GetValueOrDefault(0L),
                        Username = sendMessageRequest.ChatId.Username,
                        Type = ChatType.Private,
                    },
                    Text = sendMessageRequest.Text,
                    MessageThreadId = sendMessageRequest.MessageThreadId,
                };
                Messages.Enqueue(sendMessageRequest);
                string json = JsonSerializer.Serialize(message);
                return Task.FromResult(JsonSerializer.Deserialize<TResponse>(json)!);
            }
            default:
                throw new NotImplementedException();
        }
    }

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