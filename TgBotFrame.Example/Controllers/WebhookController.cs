using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Telegram.Bot.Types;
using TgBotFrame.Options;
using TgBotFrame.Services;

namespace TgBotFrame.Example.Controllers;

[ApiController]
[Route("tg")]
public class WebhookController(
    BotService botService,
    IOptions<TgBotOptions> options,
    ILogger<WebhookController> logger,
    IWebHostEnvironment webHostEnvironment) : ControllerBase
{
    [HttpGet]
    public ActionResult Get() => StatusCode(StatusCodes.Status405MethodNotAllowed);

    [HttpPost]
    public async Task<ActionResult> Post([FromBody] Update update, CancellationToken ct = default)
    {
        string? secretValue = options.Value.WebhookSecretToken;
        if (secretValue is not null && secretValue != HttpContext.Request.Headers["X-Telegram-Bot-Api-Secret-Token"])
        {
            return Unauthorized();
        }

        try
        {
            await botService.HandleUpdateAsync(update, ct);
        }
        catch (TaskCanceledException e)
        {
            logger.LogWarning(e, "Webhook request was cancelled");
            return Empty;
        }
        catch (Exception e)
        {
            logger.LogError(e, "Error handling update");
            if (webHostEnvironment.IsDevelopment())
            {
                return Problem(
                    "Error handling update",
                    null,
                    StatusCodes.Status500InternalServerError,
                    e.Message,
                    e.GetType().Name);
            }

            return Problem(
                "Error handling update",
                null,
                StatusCodes.Status500InternalServerError);
        }

        return Ok();
    }
}