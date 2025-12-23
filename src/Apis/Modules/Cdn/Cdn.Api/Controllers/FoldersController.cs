using Cdn.Application.Features.Folders;

namespace Cdn.Api.Controllers;

[Route($"{ApiConstants.ApiPrefix}/folders")]
[ApiController]
[Authorize]
[EnableRateLimiting(nameof(RateLimitPolicy.IdentityFixed))]
public class FoldersController(IMediator mediator) : ControllerBase
{
    [HttpGet("")]
    [ProducesResponseType(typeof(ApiResult<ListItem.Result>), (int)HttpStatusCode.OK)]
    [CheckRights(FeatureCodes.Cdn.Library, ActionCodes.View)]
    public async Task<IActionResult> ListItemAsync([FromQuery] ListItem.Request request, CancellationToken cancellationToken = default)
    {
        var result = await mediator.Send(request, cancellationToken);
        return Ok(result);
    }

    [HttpGet("trash")]
    [ProducesResponseType(typeof(ApiResult<GetItemsInTrash.Result>), (int)HttpStatusCode.OK)]
    [CheckRights(FeatureCodes.Cdn.Library, ActionCodes.View)]
    public async Task<IActionResult> GetItemsInTrashAsync([FromQuery] GetItemsInTrash.Request request, CancellationToken cancellationToken = default)
    {
        var result = await mediator.Send(request, cancellationToken);
        return Ok(result);
    }

    [HttpGet("{id}")]
    [ProducesResponseType(typeof(ApiResult<GetItem.Result>), (int)HttpStatusCode.OK)]
    [CheckRights(FeatureCodes.Cdn.Library, ActionCodes.View)]
    public async Task<IActionResult> GetItemAsync([FromRoute][ZCodeToInt64] long id,
        [FromQuery] GetItem.Request request,
        CancellationToken cancellationToken = default)
    {
        request.Id = id;
        var result = await mediator.Send(request, cancellationToken);
        return Ok(result);
    }

    [HttpPost(""), MapToApiVersion(ApiConstants.Ver1_0)]
    [ProducesResponseType(typeof(ApiResult<CreateOrUpdate.Result>), (int)HttpStatusCode.OK)]
    [CheckRights(FeatureCodes.Cdn.Library, ActionCodes.Create)]
    public async Task<IActionResult> CreateAsync([FromBody] CreateOrUpdate.Command command, CancellationToken cancellationToken = default)
    {
        var result = await mediator.Send(command, cancellationToken);
        return Ok(result);
    }

    [HttpPut("{id}"), MapToApiVersion(ApiConstants.Ver1_0)]
    [ProducesResponseType(typeof(ApiResult<CreateOrUpdate.Result>), (int)HttpStatusCode.OK)]
    [CheckRights(FeatureCodes.Cdn.Library, ActionCodes.Rename)]
    public async Task<IActionResult> UpdateAsync([FromRoute][ZCodeToInt64] long id,
        [FromBody] CreateOrUpdate.Command command, CancellationToken cancellationToken = default)
    {
        command.Id = id;
        var result = await mediator.Send(command, cancellationToken);
        return Ok(result);
    }

    [HttpPut("{id}/move"), MapToApiVersion(ApiConstants.Ver1_0)]
    [ProducesResponseType(typeof(ApiResult<Move.Result>), (int)HttpStatusCode.OK)]
    [CheckRights(FeatureCodes.Cdn.Library, ActionCodes.Move)]
    public async Task<IActionResult> MoveAsync([FromRoute][ZCodeToInt64] long id,
        [FromBody] Move.Command command, CancellationToken cancellationToken = default)
    {
        command.Id = id;
        var result = await mediator.Send(command, cancellationToken);
        return Ok(result);
    }

    [HttpPut("{id}/rename"), MapToApiVersion(ApiConstants.Ver1_0)]
    [ProducesResponseType(typeof(ApiResult<Move.Result>), (int)HttpStatusCode.OK)]
    [CheckRights(FeatureCodes.Cdn.Library, ActionCodes.Rename)]
    public async Task<IActionResult> RenameAsync([FromRoute][ZCodeToInt64] long id,
        [FromBody] Rename.Command command, CancellationToken cancellationToken = default)
    {
        command.Id = id;
        var result = await mediator.Send(command, cancellationToken);
        return Ok(result);
    }

    [HttpPut("{id}/move-to-trash"), MapToApiVersion(ApiConstants.Ver1_0)]
    [ProducesResponseType(typeof(ApiResult<MoveToTrash.Result>), (int)HttpStatusCode.OK)]
    [CheckRights(FeatureCodes.Cdn.Library, ActionCodes.Delete)]
    public async Task<IActionResult> MoveToTrashAsync([FromRoute][ZCodeToInt64] long id,
        [FromBody] MoveToTrash.Command command, CancellationToken cancellationToken = default)
    {
        command.Id = id;
        var result = await mediator.Send(command, cancellationToken);
        return Ok(result);
    }

    [HttpPut("{id}/restore-from-trash"), MapToApiVersion(ApiConstants.Ver1_0)]
    [ProducesResponseType(typeof(ApiResult<RestoreFromTrash.Result>), (int)HttpStatusCode.OK)]
    [CheckRights(FeatureCodes.Cdn.Library, ActionCodes.Move)]
    public async Task<IActionResult> RestoreFromTrashAsync([FromRoute][ZCodeToInt64] long id,
        [FromBody] RestoreFromTrash.Command command, CancellationToken cancellationToken = default)
    {
        command.Id = id;
        var result = await mediator.Send(command, cancellationToken);
        return Ok(result);
    }

    [HttpDelete("{id}")]
    [ProducesResponseType(typeof(ApiResult<Delete.Result>), (int)HttpStatusCode.OK)]
    [CheckRights(FeatureCodes.Cdn.Library, ActionCodes.Delete)]
    public async Task<IActionResult> DeleteAsync([FromRoute][ZCodeToInt64] long id,
        [FromBody] Delete.Command command,
        CancellationToken cancellationToken = default)
    {
        command.Id = id;
        var result = await mediator.Send(command, cancellationToken);
        return Ok(result);
    }
}