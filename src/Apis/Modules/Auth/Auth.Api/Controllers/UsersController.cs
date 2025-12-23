using Auth.Application.Features.Users;

namespace Auth.Api.Controllers;

[Route($"{ApiConstants.ApiPrefix}/users")]
[ApiController]
[Authorize]
[EnableRateLimiting(nameof(RateLimitPolicy.IdentityFixed))]
public class UsersController(IMediator mediator) : ControllerBase
{
    [HttpGet(""), MapToApiVersion(ApiConstants.Ver1_0)]
    [ProducesResponseType(typeof(ApiResult<ListItem.Result>), (int)HttpStatusCode.OK)]
    [CheckRights(FeatureCodes.Administration.Users, ActionCodes.View)]
    public async Task<IActionResult> ListAsync([FromQuery] ListItem.Request request, CancellationToken cancellationToken = default)
    {
        var result = await mediator.Send(request, cancellationToken);
        return Ok(result);
    }

    [HttpGet("{id}"), MapToApiVersion(ApiConstants.Ver1_0)]
    [ProducesResponseType(typeof(ApiResult<GetItem.Result>), (int)HttpStatusCode.OK)]
    [CheckRights(FeatureCodes.Administration.Users, ActionCodes.View)]
    public async Task<IActionResult> GetAsync([FromRoute] GetItem.Request request, CancellationToken cancellationToken = default)
    {
        var result = await mediator.Send(request, cancellationToken);
        return Ok(result);
    }

    [HttpPost(""), MapToApiVersion(ApiConstants.Ver1_0)]
    [ProducesResponseType(typeof(ApiResult<CreateOrUpdate.Result>), (int)HttpStatusCode.OK)]
    [CheckRights(FeatureCodes.Administration.Users, ActionCodes.Create)]
    public async Task<IActionResult> CreateAsync([FromBody] CreateOrUpdate.Command command, CancellationToken cancellationToken = default)
    {
        var result = await mediator.Send(command, cancellationToken);
        return Ok(result);
    }

    [HttpPut("{id}"), MapToApiVersion(ApiConstants.Ver1_0)]
    [ProducesResponseType(typeof(ApiResult<CreateOrUpdate.Result>), (int)HttpStatusCode.OK)]
    [CheckRights(FeatureCodes.Administration.Users, ActionCodes.Update)]
    public async Task<IActionResult> UpdateAsync([FromRoute][ZCodeToInt64] long id,
        [FromBody] CreateOrUpdate.Command command, CancellationToken cancellationToken = default)
    {
        command.Id = id;
        var result = await mediator.Send(command, cancellationToken);
        return Ok(result);
    }

    [HttpDelete("{id}"), MapToApiVersion(ApiConstants.Ver1_0)]
    [ProducesResponseType(typeof(ApiResult<Delete.Result>), (int)HttpStatusCode.OK)]
    [CheckRights(FeatureCodes.Administration.Users, ActionCodes.Delete)]
    public async Task<IActionResult> DeleteAsync([FromRoute][ZCodeToInt64] long id,
        [FromBody] Delete.Command command,
        CancellationToken cancellationToken = default)
    {
        command.Id = id;
        var result = await mediator.Send(command, cancellationToken);
        return Ok(result);
    }

    [HttpPut("{id}/reset-password"), MapToApiVersion(ApiConstants.Ver1_0)]
    [ProducesResponseType(typeof(ApiResult<ResetPassword.Result>), (int)HttpStatusCode.OK)]
    [CheckRights(FeatureCodes.Administration.Users, ActionCodes.ResetPassword)]
    public async Task<IActionResult> ResetPasswordAsync([FromRoute][ZCodeToInt64] long id,
        [FromBody] ResetPassword.Command command, CancellationToken cancellationToken = default)
    {
        command.UserId = id;
        var result = await mediator.Send(command, cancellationToken);
        return Ok(result);
    }

    [HttpGet("sessions"), MapToApiVersion(ApiConstants.Ver1_0)]
    [ProducesResponseType(typeof(ApiResult<GetUserSessions.Result>), (int)HttpStatusCode.OK)]
    [CheckRights(FeatureCodes.Administration.UserSessions, ActionCodes.View)]
    public async Task<IActionResult> GetUserSessionsAsync([FromQuery] GetUserSessions.Request request, CancellationToken cancellationToken = default)
    {
        var result = await mediator.Send(request, cancellationToken);
        return Ok(result);
    }
}