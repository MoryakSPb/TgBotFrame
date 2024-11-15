namespace TgBotFrame.Middleware;

/// <summary>
/// Базовый класс для ПО промежуточного слоя
/// </summary>
public abstract class FrameMiddleware : IDisposable
{
    /// <summary>
    /// Делегат, представляющий вызов InvokeAsync следующего ПО промежуточного слоя
    /// </summary>
    protected internal FrameUpdateDelegate Next { get; internal set; } = null!;
    public static FrameUpdateDelegate Empty { get; } = (_, _, _) => Task.CompletedTask;

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    /// <summary>
    /// Метод, определенный в наследниках, выполняющий необходимые действия. Вызов Next необходим для перехода к следующему ПО промежуточного соля 
    /// </summary>
    /// <param name="update">Обновление Telegram</param>
    /// <param name="context">Объект с сохраненным состоянием обработки обновления</param>
    /// <param name="ct">Токен отмены</param>
    /// <returns></returns>
    public abstract Task InvokeAsync(Update update, FrameContext context, CancellationToken ct = default);

    protected virtual void Dispose(bool disposing)
    {
        if (disposing)
        {
            Next = null!;
        }
    }
}