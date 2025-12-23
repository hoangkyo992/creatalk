using Cms.Application.Features.Attendees;

namespace Cms.Api.Controllers;

[Route($"{ApiConstants.ApiPrefix}/attendees")]
[ApiController]
[Authorize]
[EnableRateLimiting(nameof(RateLimitPolicy.IdentityFixed))]
public class AttendeesController(IMediator mediator) : ControllerBase
{
    [HttpGet(""), MapToApiVersion(ApiConstants.Ver1_0)]
    [ProducesResponseType(typeof(ApiResult<ListItem.Result>), (int)HttpStatusCode.OK)]
    [CheckRights(FeatureCodes.Cms.Attendees, ActionCodes.View)]
    public async Task<IActionResult> ListAsync([FromQuery] ListItem.Request request)
    {
        var result = await mediator.Send(request);
        return Ok(result);
    }

    [HttpGet("{id}"), MapToApiVersion(ApiConstants.Ver1_0)]
    [ProducesResponseType(typeof(ApiResult<GetItem.Result>), (int)HttpStatusCode.OK)]
    [CheckRights(FeatureCodes.Cms.Attendees, ActionCodes.View)]
    public async Task<IActionResult> GetAsync([FromRoute] GetItem.Request request)
    {
        var result = await mediator.Send(request);
        return Ok(result);
    }

    [HttpDelete("{id}"), MapToApiVersion(ApiConstants.Ver1_0)]
    [ProducesResponseType(typeof(ApiResult<Delete.Result>), (int)HttpStatusCode.OK)]
    [CheckRights(FeatureCodes.Cms.Attendees, ActionCodes.Delete)]
    public async Task<IActionResult> DeleteAsync([FromRoute] Delete.Command command)
    {
        var result = await mediator.Send(command);
        return Ok(result);
    }
}