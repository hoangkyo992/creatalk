using Auth.Application.Features.Settings;
using Auth.Application.Features.Settings.Dtos;

namespace Auth.Api.Controllers;

[Route($"{ApiConstants.ApiPrefix}/settings")]
[ApiController]
[Authorize]
[EnableRateLimiting(nameof(RateLimitPolicy.IdentityFixed))]
public class SettingsController(IMediator mediator) : ControllerBase
{
    [HttpGet(""), MapToApiVersion(ApiConstants.Ver1_0)]
    [ProducesResponseType(typeof(ApiResult<ListItem.Result>), (int)HttpStatusCode.OK)]
    [CheckRights(FeatureCodes.Administration.Settings, ActionCodes.View)]
    public async Task<IActionResult> ListAsync([FromQuery] ListItem.Request request, CancellationToken cancellationToken = default)
    {
        var result = await mediator.Send(request, cancellationToken);
        return Ok(result);
    }

    [HttpGet("{id}"), MapToApiVersion(ApiConstants.Ver1_0)]
    [ProducesResponseType(typeof(ApiResult<SettingResDto>), (int)HttpStatusCode.OK)]
    [CheckRights(FeatureCodes.Administration.Settings, ActionCodes.View)]
    public async Task<IActionResult> GetAsync([FromRoute] GetItem.Request request, CancellationToken cancellationToken = default)
    {
        var result = await mediator.Send(request, cancellationToken);
        return Ok(result);
    }

    [HttpGet("key"), MapToApiVersion(ApiConstants.Ver1_0)]
    [ProducesResponseType(typeof(ApiResult<SettingResDto?>), (int)HttpStatusCode.OK)]
    [CheckRights(FeatureCodes.Administration.Settings, ActionCodes.View)]
    public async Task<IActionResult> GetByKeyAsync([FromQuery] GetByKey.Request request, CancellationToken cancellationToken = default)
    {
        var result = await mediator.Send(request, cancellationToken);
        return Ok(result);
    }

    [HttpPost(""), MapToApiVersion(ApiConstants.Ver1_0)]
    [ProducesResponseType(typeof(ApiResult<CreateOrUpdate.Result>), (int)HttpStatusCode.OK)]
    [CheckRights(FeatureCodes.Administration.Settings, ActionCodes.Create)]
    public async Task<IActionResult> CreateAsync([FromBody] CreateOrUpdate.Command command, CancellationToken cancellationToken = default)
    {
        var result = await mediator.Send(command, cancellationToken);
        return Ok(result);
    }

    [HttpPut("{id}"), MapToApiVersion(ApiConstants.Ver1_0)]
    [ProducesResponseType(typeof(ApiResult<CreateOrUpdate.Result>), (int)HttpStatusCode.OK)]
    [CheckRights(FeatureCodes.Administration.Settings, ActionCodes.Update)]
    public async Task<IActionResult> UpdateAsync([FromRoute][ZCodeToInt64] long id,
        [FromBody] CreateOrUpdate.Command command, CancellationToken cancellationToken = default)
    {
        command.Id = id;
        var result = await mediator.Send(command, cancellationToken);
        return Ok(result);
    }

    [HttpDelete("{id}"), MapToApiVersion(ApiConstants.Ver1_0)]
    [ProducesResponseType(typeof(ApiResult<Delete.Result>), (int)HttpStatusCode.OK)]
    [CheckRights(FeatureCodes.Administration.Settings, ActionCodes.Delete)]
    public async Task<IActionResult> DeleteAsync([FromRoute][ZCodeToInt64] long id,
        [FromBody] Delete.Command command,
        CancellationToken cancellationToken = default)
    {
        command.Id = id;
        var result = await mediator.Send(command, cancellationToken);
        return Ok(result);
    }
}