namespace Cms.Api.Controllers;

[Route($"webhooks")]
[ApiController]
[AllowAnonymous]
[EnableRateLimiting(nameof(RateLimitPolicy.IpAddressTokenBucket))]
public class WebhooksController(IMediator mediator) : ControllerBase
{
    [HttpGet("zns/{providerId}/events"), MapToApiVersion(ApiConstants.Ver1_0)]
    public async Task<IActionResult> CapturePaypalEventAsync([FromRoute][ZCodeToInt64] long providerId,
        [FromQuery(Name = "t")] string token)
    {
        if (!VerifyToken(token, providerId))
            return BadRequest("[1000] Invalid token!!!");

        var headers = HttpContext.Request.Headers.ToDictionary(c => c.Key.ToLowerInvariant(), c => c.Value.ToString());

        var server = headers.GetValueOrDefault("x-zevent-server");
        if (server != "ZNS")
        {
            return BadRequest($"[1003] Invalid request!");
        }
        var signature = headers.GetValueOrDefault("x-zevent-signature");
        var payload = await new StreamReader(HttpContext.Request.Body).ReadToEndAsync();

        Response.OnCompleted(async () =>
        {
            // Handle logic
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