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

    [HttpPut("{id}/phone-number"), MapToApiVersion(ApiConstants.Ver1_0)]
    [ProducesResponseType(typeof(ApiResult<UpdatePhoneNumber.Result>), (int)HttpStatusCode.OK)]
    [CheckRights(FeatureCodes.Cms.Attendees, ActionCodes.Update)]
    public async Task<IActionResult> UpdatePhoneNumberAsync([FromRoute][ZCodeToInt64] long id,
        [FromBody] UpdatePhoneNumber.Command command, CancellationToken cancellationToken = default)
    {
        command.Id = id;
        var result = await mediator.Send(command, cancellationToken);
        return Ok(result);
    }

    [HttpPost("{id}/cancel"), MapToApiVersion(ApiConstants.Ver1_0)]
    [ProducesResponseType(typeof(ApiResult<Cancel.Result>), (int)HttpStatusCode.OK)]
    [CheckRights(FeatureCodes.Cms.Attendees, ActionCodes.Update)]
    public async Task<IActionResult> CancelAsync([FromRoute][ZCodeToInt64] long id,
        [FromBody] Cancel.Command command, CancellationToken cancellationToken = default)
    {
        command.Id = id;
        var result = await mediator.Send(command, cancellationToken);
        return Ok(result);
    }

    [HttpPost("{id}/messages/resend"), MapToApiVersion(ApiConstants.Ver1_0)]
    [ProducesResponseType(typeof(ApiResult<ResendMessage.Result>), (int)HttpStatusCode.OK)]
    [CheckRights(FeatureCodes.Cms.Attendees, ActionCodes.CreateMessages)]
    public async Task<IActionResult> ResendMessageAsync([FromRoute][ZCodeToInt64] long id,
        [FromBody] ResendMessage.Command command, CancellationToken cancellationToken = default)
    {
        command.Id = id;
        var result = await mediator.Send(command, cancellationToken);
        return Ok(result);
    }

    [HttpPost("messages"), MapToApiVersion(ApiConstants.Ver1_0)]
    [ProducesResponseType(typeof(ApiResult<CreateMessages.Result>), (int)HttpStatusCode.OK)]
    [CheckRights(FeatureCodes.Cms.Attendees, ActionCodes.CreateMessages)]
    public async Task<IActionResult> CreateMessagesAsync([FromBody] CreateMessages.Command command)
    {
        var result = await mediator.Send(command);
        return Ok(result);
    }
}