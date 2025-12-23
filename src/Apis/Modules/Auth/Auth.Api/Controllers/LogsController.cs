using Auth.Application.Features.Loggings;

namespace Auth.Api.Controllers;

/// <summary>
/// Log controller
/// </summary>
/// <param name="mediator"></param>
[Route($"{ApiConstants.ApiPrefix}/logs")]
[ApiController]
[Authorize]
[EnableRateLimiting(nameof(RateLimitPolicy.IdentityFixed))]
public class LogsController(IMediator mediator) : ControllerBase
{
    /// <summary>
    /// Get activity logs
    /// </summary>
    /// <param name="request"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    [HttpGet("activities"), MapToApiVersion(ApiConstants.Ver1_0)]
    [ProducesResponseType(typeof(ApiResult<Activities.Result>), (int)HttpStatusCode.OK)]
    [CheckRights(FeatureCodes.Administration.Activities, ActionCodes.View)]
    public async Task<IActionResult> GetActivitiesAsync([FromQuery] Activities.Request request, CancellationToken cancellationToken = default)
    {
        var result = await mediator.Send(request, cancellationToken);
        return Ok(result);
    }
}