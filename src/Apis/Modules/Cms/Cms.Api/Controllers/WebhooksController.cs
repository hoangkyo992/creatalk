using Cms.Application.Features.Webhooks;

namespace Cms.Api.Controllers;

[Route($"webhooks")]
[ApiController]
[AllowAnonymous]
[EnableRateLimiting(nameof(RateLimitPolicy.IpAddressTokenBucket))]
public class WebhooksController(IMediator mediator, ILogger<WebhooksController> logger) : ControllerBase
{
    [HttpPost("zns/{providerId}/events"), MapToApiVersion(ApiConstants.Ver1_0)]
    public async Task<IActionResult> CaptureZnsEventAsync([FromRoute][ZCodeToInt64] long providerId,
        [FromQuery(Name = "t")] string token)
    {
        if (!VerifyToken(token, providerId))
            return BadRequest("[1000] Invalid token!!!");

        var headers = HttpContext.Request.Headers.ToDictionary(c => c.Key.ToLowerInvariant(), c => c.Value.ToString());

        var server = headers.GetValueOrDefault("x-zevent-server");
        if (server != "ZNS")
        {
            return BadRequest($"[1003] Invalid server!");
        }
        var signature = headers.GetValueOrDefault("x-zevent-signature") ?? string.Empty;
        if (string.IsNullOrWhiteSpace(signature))
        {
            return BadRequest($"[1004] Invalid signature!");
        }

        var payload = await new StreamReader(HttpContext.Request.Body).ReadToEndAsync();

        Response.OnCompleted(async () =>
        {
            var validationResult = await mediator.Send(new VerifyEvent.Command
            {
                ProviderCode = "ZNS",
                ProviderId = providerId,
                EventPayload = payload,
                Parameters = new Dictionary<string, object>
                {
                    { "Signature", signature }
                }
            });

            if (!validationResult.IsSuccess)
            {
                logger.LogInformation("[1005] Invalid request!");
                return;
            }

            if (validationResult.Result.EventName == "user_received_message")
            {
                await mediator.Send(new HandleUserReceivedMessageEvent.Command
                {
                    ProviderCode = "ZNS",
                    ProviderId = providerId,
                    Parameters = new Dictionary<string, object>
                    {
                        { "Signature", signature }
                    },
                    EventName = validationResult.Result.EventName,
                    EventPayload = payload
                }, CancellationToken.None);
            }
        });

        return Ok("OK");
    }

    private static bool VerifyToken(string token, long providerId)
    {
        if (string.IsNullOrWhiteSpace(token)
           || !ZCode.TryGetInt64(token, out var val)
           || val != (long.MaxValue - providerId))
            return false;
        return true;
    }
}