using Cdn.Application.Features.Files;

namespace Cdn.Api.Controllers;

[Route($"{ApiConstants.ApiPrefix}/files")]
[ApiController]
[Authorize]
[EnableRateLimiting(nameof(RateLimitPolicy.IdentityFixed))]
public class FilesController(IMediator mediator) : ControllerBase
{
    [HttpGet("statistics")]
    [ProducesResponseType(typeof(ApiResult<Statistics.Result>), (int)HttpStatusCode.OK)]
    [CheckRights(FeatureCodes.Cdn.Library, ActionCodes.View)]
    public async Task<IActionResult> GetStatisticsAsync([FromQuery] Statistics.Request request, CancellationToken cancellationToken = default)
    {
        var result = await mediator.Send(request, cancellationToken);
        return Ok(result);
    }

    [HttpGet(""), MapToApiVersion(ApiConstants.Ver1_0)]
    [ProducesResponseType(typeof(ApiResult<ListItem.Result>), (int)HttpStatusCode.OK)]
    [CheckRights(FeatureCodes.Cdn.Library, ActionCodes.View)]
    public async Task<IActionResult> ListAsync([FromQuery] ListItem.Request request)
    {
        var result = await mediator.Send(request);
        return Ok(result);
    }

    [HttpGet("{id}")]
    [ProducesResponseType(typeof(ApiResult<GetItem.Result>), (int)HttpStatusCode.OK)]
    [CheckRights(FeatureCodes.Cdn.Library, ActionCodes.View)]
    public async Task<IActionResult> GetItemAsync([FromRoute][ZCodeToInt64] long id,
        [FromQuery] GetItem.Request request, CancellationToken cancellationToken = default)
    {
        request.Id = id;
        var result = await mediator.Send(request, cancellationToken);
        return Ok(result);
    }

    [HttpPost("upload")]
    [ProducesResponseType(typeof(ApiResult<Upload.Result>), (int)HttpStatusCode.OK)]
    [CheckRights(FeatureCodes.Cdn.Library, ActionCodes.Upload, ActionCodes.Import)]
    public async Task<IActionResult> UploadAsync(IEnumerable<IFormFile> files,
        [ZCodeToInt64] long? folderId,
        CancellationToken cancellationToken = default)
    {
        var command = new Upload.Command(files, folderId);
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

    [HttpDelete("{id}"), MapToApiVersion(ApiConstants.Ver1_0)]
    [ProducesResponseType(typeof(ApiResult<Delete.Result>), (int)HttpStatusCode.OK)]
    [CheckRights(FeatureCodes.Cdn.Library, ActionCodes.Delete)]
    public async Task<IActionResult> DeleteOneAsync([FromRoute] Delete.Command command,
        CancellationToken cancellationToken = default)
    {
        var result = await mediator.Send(command, cancellationToken);
        return Ok(result);
    }
}