using Auth.Application.Features.Roles;

namespace Auth.Api.Controllers;

[Route($"{ApiConstants.ApiPrefix}/roles")]
[ApiController]
[Authorize]
[EnableRateLimiting(nameof(RateLimitPolicy.IdentityFixed))]
public class RolesController(IMediator mediator) : ControllerBase
{
    [HttpGet(""), MapToApiVersion(ApiConstants.Ver1_0)]
    [ProducesResponseType(typeof(ApiResult<ListItem.Result>), (int)HttpStatusCode.OK)]
    [CheckRights(FeatureCodes.Administration.Roles, ActionCodes.View)]
    [CheckRights(FeatureCodes.Administration.Users, ActionCodes.View)]
    public async Task<IActionResult> ListAsync([FromQuery] ListItem.Request request, CancellationToken cancellationToken = default)
    {
        var result = await mediator.Send(request, cancellationToken);
        return Ok(result);
    }

    [HttpGet("{id}"), MapToApiVersion(ApiConstants.Ver1_0)]
    [ProducesResponseType(typeof(ApiResult<GetItem.Result>), (int)HttpStatusCode.OK)]
    [CheckRights(FeatureCodes.Administration.Roles, ActionCodes.View)]
    public async Task<IActionResult> GetAsync([FromRoute] GetItem.Request request, CancellationToken cancellationToken = default)
    {
        var result = await mediator.Send(request, cancellationToken);
        return Ok(result);
    }

    [HttpPost(""), MapToApiVersion(ApiConstants.Ver1_0)]
    [ProducesResponseType(typeof(ApiResult<CreateOrUpdate.Result>), (int)HttpStatusCode.OK)]
    [CheckRights(FeatureCodes.Administration.Roles, ActionCodes.Create)]
    public async Task<IActionResult> CreateAsync([FromBody] CreateOrUpdate.Command command, CancellationToken cancellationToken = default)
    {
        var result = await mediator.Send(command, cancellationToken);
        return Ok(result);
    }

    [HttpPut("{id}"), MapToApiVersion(ApiConstants.Ver1_0)]
    [ProducesResponseType(typeof(ApiResult<CreateOrUpdate.Result>), (int)HttpStatusCode.OK)]
    [CheckRights(FeatureCodes.Administration.Roles, ActionCodes.Update)]
    public async Task<IActionResult> UpdateAsync([FromRoute][ZCodeToInt64] long id,
        [FromBody] CreateOrUpdate.Command command, CancellationToken cancellationToken = default)
    {
        command.Id = id;
        var result = await mediator.Send(command, cancellationToken);
        return Ok(result);
    }

    [HttpGet("{id}/features"), MapToApiVersion(ApiConstants.Ver1_0)]
    [ProducesResponseType(typeof(ApiResult<GetFeatures.Result>), (int)HttpStatusCode.OK)]
    [CheckRights(FeatureCodes.Administration.Roles, ActionCodes.GrantAccess)]
    public async Task<IActionResult> GetFeaturesAsync([FromRoute] GetFeatures.Request request, CancellationToken cancellationToken = default)
    {
        var result = await mediator.Send(request, cancellationToken);
        return Ok(result);
    }

    [HttpPost("{id}/features"), MapToApiVersion(ApiConstants.Ver1_0)]
    [ProducesResponseType(typeof(ApiResult<UpdateFeatures.Result>), (int)HttpStatusCode.OK)]
    [CheckRights(FeatureCodes.Administration.Roles, ActionCodes.GrantAccess)]
    public async Task<IActionResult> UpdateFeaturesAsync([FromRoute][ZCodeToInt64] long id,
        [FromBody] UpdateFeatures.Command command, CancellationToken cancellationToken = default)
    {
        command.Id = id;
        var result = await mediator.Send(command, cancellationToken);
        return Ok(result);
    }

    [HttpDelete("{id}"), MapToApiVersion(ApiConstants.Ver1_0)]
    [ProducesResponseType(typeof(ApiResult<Delete.Result>), (int)HttpStatusCode.OK)]
    [CheckRights(FeatureCodes.Administration.Roles, ActionCodes.Delete)]
    public async Task<IActionResult> DeleteAsync([FromRoute][ZCodeToInt64] long id,
        [FromBody] Delete.Command command,
        CancellationToken cancellationToken = default)
    {
        command.Id = id;

        var result = await mediator.Send(command, cancellationToken);
        return Ok(result);
    }
}