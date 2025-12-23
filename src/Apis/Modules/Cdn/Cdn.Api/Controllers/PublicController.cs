using Cdn.Application.Features.Public;

namespace Cdn.Api.Controllers;

[Route($"{ApiConstants.ApiPrefix}/public")]
[ApiController]
[AllowAnonymous]
public class PublicController(IMediator mediator) : ControllerBase
{
    [HttpGet("files"), MapToApiVersion(ApiConstants.Ver1_0)]
    [ProducesResponseType(typeof(ApiResult<GetFolderItems.Result>), (int)HttpStatusCode.OK)]
    [ResponseCache(CacheProfileName = "Default", Location = ResponseCacheLocation.Any, VaryByQueryKeys = ["folderId"])]
    [EnableRateLimiting(nameof(RateLimitPolicy.IpAddressFixed))]
    public async Task<IActionResult> GetFolderItemsAsync([FromQuery] GetFolderItems.Request request,
        CancellationToken cancellationToken = default)
    {
        var result = await mediator.Send(request, cancellationToken);
        return Ok(result);
    }

    [HttpGet("albums"), MapToApiVersion(ApiConstants.Ver1_0)]
    [ProducesResponseType(typeof(ApiResult<GetAlbumItems.Result>), (int)HttpStatusCode.OK)]
    [ResponseCache(CacheProfileName = "Default", Location = ResponseCacheLocation.Any, VaryByQueryKeys = ["album"])]
    [EnableRateLimiting(nameof(RateLimitPolicy.IpAddressFixed))]
    public async Task<IActionResult> GetAlbumItemsAsync([FromQuery] GetAlbumItems.Request request,
        CancellationToken cancellationToken = default)
    {
        var result = await mediator.Send(request, cancellationToken);
        return Ok(result);
    }
}