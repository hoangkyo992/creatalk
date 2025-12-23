using Auth.Application.Features.Settings;

namespace Auth.Api.Controllers;

[Route($"{ApiConstants.ApiPrefix}/public")]
[ApiController]
[AllowAnonymous]
public class PublicController(IMediator mediator) : ControllerBase
{
    [HttpGet("settings"), MapToApiVersion(ApiConstants.Ver1_0)]
    [ProducesResponseType(typeof(ApiResult<ListItem.Result>), (int)HttpStatusCode.OK)]
    [CheckRights(FeatureCodes.Administration.Settings, ActionCodes.View)]
    [EnableRateLimiting(nameof(RateLimitPolicy.IpAddressFixed))]
    public async Task<IActionResult> GetSettingsAsync([FromQuery] ListItem.Request request, CancellationToken cancellationToken = default)
    {
        var result = await mediator.Send(request, cancellationToken);
        return Ok(result);
    }
}