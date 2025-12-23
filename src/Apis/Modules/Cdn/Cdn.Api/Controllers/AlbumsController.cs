using Cdn.Application.Features.Albums;

namespace Cdn.Api.Controllers;

[Route($"{ApiConstants.ApiPrefix}/albums")]
[ApiController]
[Authorize]
[EnableRateLimiting(nameof(RateLimitPolicy.IdentityFixed))]
public class AlbumsController(IMediator mediator) : ControllerBase
{
    [HttpGet("")]
    [ProducesResponseType(typeof(ApiResult<ListItem.Result>), (int)HttpStatusCode.OK)]
    [CheckRights(FeatureCodes.Cdn.Albums, ActionCodes.View)]
    public async Task<IActionResult> ListItemAsync([FromQuery] ListItem.Request request, CancellationToken cancellationToken = default)
    {
        var result = await mediator.Send(request, cancellationToken);
        return Ok(result);
    }

    [HttpGet("{id}")]
    [ProducesResponseType(typeof(ApiResult<GetItem.Result>), (int)HttpStatusCode.OK)]
    [CheckRights(FeatureCodes.Cdn.Albums, ActionCodes.View)]
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
    [CheckRights(FeatureCodes.Cdn.Albums, ActionCodes.Create)]
    public async Task<IActionResult> CreateAsync([FromBody] CreateOrUpdate.Command command, CancellationToken cancellationToken = default)
    {
        var result = await mediator.Send(command, cancellationToken);
        return Ok(result);
    }

    [HttpPut("{id}"), MapToApiVersion(ApiConstants.Ver1_0)]
    [ProducesResponseType(typeof(ApiResult<CreateOrUpdate.Result>), (int)HttpStatusCode.OK)]
    [CheckRights(FeatureCodes.Cdn.Albums, ActionCodes.Update)]
    public async Task<IActionResult> UpdateAsync([FromRoute][ZCodeToInt64] long id,
        [FromBody] CreateOrUpdate.Command command, CancellationToken cancellationToken = default)
    {
        command.Id = id;
        var result = await mediator.Send(command, cancellationToken);
        return Ok(result);
    }

    [HttpDelete("{id}")]
    [ProducesResponseType(typeof(ApiResult<Delete.Result>), (int)HttpStatusCode.OK)]
    [CheckRights(FeatureCodes.Cdn.Albums, ActionCodes.Delete)]
    public async Task<IActionResult> DeleteAsync([FromRoute][ZCodeToInt64] long id,
        [FromBody] Delete.Command command,
        CancellationToken cancellationToken = default)
    {
        command.Id = id;
        var result = await mediator.Send(command, cancellationToken);
        return Ok(result);
    }

    [HttpGet("{id}/files"), MapToApiVersion(ApiConstants.Ver1_0)]
    [ProducesResponseType(typeof(ApiResult<GetFiles.Result>), (int)HttpStatusCode.OK)]
    [CheckRights(FeatureCodes.Cdn.Albums, ActionCodes.View)]
    public async Task<IActionResult> GetFilesAsync([FromRoute][ZCodeToInt64] long id,
        [FromQuery] GetFiles.Request request, CancellationToken cancellationToken = default)
    {
        request.Id = id;
        var result = await mediator.Send(request, cancellationToken);
        return Ok(result);
    }

    [HttpPost("{id}/files"), MapToApiVersion(ApiConstants.Ver1_0)]
    [ProducesResponseType(typeof(ApiResult<AddFiles.Result>), (int)HttpStatusCode.OK)]
    [CheckRights(FeatureCodes.Cdn.Albums, ActionCodes.Update)]
    public async Task<IActionResult> AddFilesAsync([FromRoute][ZCodeToInt64] long id,
        [FromBody] AddFiles.Command command, CancellationToken cancellationToken = default)
    {
        command.Id = id;
        var result = await mediator.Send(command, cancellationToken);
        return Ok(result);
    }

    [HttpPost("{id}/files/positions"), MapToApiVersion(ApiConstants.Ver1_0)]
    [ProducesResponseType(typeof(ApiResult<UpdatePositions.Result>), (int)HttpStatusCode.OK)]
    [CheckRights(FeatureCodes.Cdn.Albums, ActionCodes.Update)]
    public async Task<IActionResult> UpdatePositionsAsync([FromRoute][ZCodeToInt64] long id,
        [FromBody] UpdatePositions.Command command, CancellationToken cancellationToken = default)
    {
        command.Id = id;
        var result = await mediator.Send(command, cancellationToken);
        return Ok(result);
    }

    [HttpDelete("{id}/files"), MapToApiVersion(ApiConstants.Ver1_0)]
    [ProducesResponseType(typeof(ApiResult<RemoveFiles.Result>), (int)HttpStatusCode.OK)]
    [CheckRights(FeatureCodes.Cdn.Albums, ActionCodes.Update)]
    public async Task<IActionResult> RemoveFilesAsync([FromRoute][ZCodeToInt64] long id,
        [FromBody] RemoveFiles.Command command, CancellationToken cancellationToken = default)
    {
        command.Id = id;
        var result = await mediator.Send(command, cancellationToken);
        return Ok(result);
    }

    [HttpPut("{id}/files/{fileId}"), MapToApiVersion(ApiConstants.Ver1_0)]
    [ProducesResponseType(typeof(ApiResult<UpdateFile.Result>), (int)HttpStatusCode.OK)]
    [CheckRights(FeatureCodes.Cdn.Albums, ActionCodes.Update)]
    public async Task<IActionResult> UpdateFileAsync([FromRoute][ZCodeToInt64] long id,
        [FromRoute][ZCodeToInt64] long fileId,
        [FromBody] UpdateFile.Command command, CancellationToken cancellationToken = default)
    {
        command.AlbumId = id;
        command.FileId = fileId;
        var result = await mediator.Send(command, cancellationToken);
        return Ok(result);
    }
}