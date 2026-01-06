using Cdn.Application;
using Cdn.Application.Features.Files;

namespace Cdn.Api.Controllers;

[Route("media")]
[ApiController]
public class MediaController(IMediator mediator, IMemoryCache cache) : ControllerBase
{
    private static readonly MemoryCacheEntryOptions CacheOptions = new MemoryCacheEntryOptions()
        .SetSlidingExpiration(TimeSpan.FromSeconds(30))
        .SetAbsoluteExpiration(TimeSpan.FromSeconds(3600))
        .SetPriority(CacheItemPriority.Normal);

    [HttpGet("{path}")]
    [AllowAnonymous]
    [ResponseCache(CacheProfileName = "Default", Location = ResponseCacheLocation.Any, VaryByQueryKeys = ["*"])]
    [EnableRateLimiting(nameof(RateLimitPolicy.IpAddressFixed))]
    public async Task<IActionResult> GetByName([FromRoute] string path,
        ImageSize? size = null,
        string? convertTo = null,
        bool crop = false,
        int top = 0,
        int left = 0,
        CancellationToken cancellationToken = default)
    {
        size ??= ImageSize.Original;
        var cacheKey = $"{Assembly.Name}-{nameof(MediaController)}-{nameof(GetByName)}-{path}-{size}-{crop}-{top}-{left}-{convertTo}";
        if (!cache.TryGetValue(cacheKey, out GetContent.Result? file))
        {
            var result = await mediator.Send(new GetContent.Request
            {
                Path = path,
                Size = size.Value,
                ConvertTo = convertTo,
                Crop = crop,
                OffsetX = left,
                OffsetY = top,
            }, cancellationToken);
            file = result.Result;
            cache.Set(cacheKey, file, CacheOptions);
        }
        return FileContent(file!);
    }

    /// <summary>
    ///
    /// </summary>
    /// <param name="id"></param>
    /// <param name="size"></param>
    /// <param name="convertTo"></param>
    /// <param name="crop"></param>
    /// <param name="top"></param>
    /// <param name="left"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    [HttpGet("files/{id}")]
    [AllowAnonymous]
    [ResponseCache(CacheProfileName = "Default", Location = ResponseCacheLocation.Any, VaryByQueryKeys = ["*"])]
    [EnableRateLimiting(nameof(RateLimitPolicy.IpAddressFixed))]
    public async Task<IActionResult> GetById([FromRoute][ZCodeToInt64] long id,
        ImageSize? size = null,
        string? convertTo = null,
        bool crop = false,
        int top = 0,
        int left = 0,
        CancellationToken cancellationToken = default)
    {
        size ??= ImageSize.Original;
        var cacheKey = $"{Assembly.Name}-{nameof(MediaController)}-{nameof(GetById)}-{id}-{size}-{crop}-{top}-{left}-{convertTo}";
        if (!cache.TryGetValue(cacheKey, out GetContent.Result? file))
        {
            var result = await mediator.Send(new GetContent.Request
            {
                Id = id,
                Size = size.Value,
                ConvertTo = convertTo,
                Crop = crop,
                OffsetX = left,
                OffsetY = top,
            }, cancellationToken);
            file = result.Result;
            cache.Set(cacheKey, file, CacheOptions);
        }

        return FileContent(file!);
    }

    private FileContentResult FileContent(GetContent.Result file)
    {
        Response.Headers.Append("Content-Disposition", new ContentDisposition
        {
            Inline = true,
            FileName = WebUtility.UrlEncode(file!.Name),
            CreationDate = file.CreatedTime,
            Size = file.Content.Length
        }.ToString());

        return File(file.Content, file.MineType ?? "application/octet-stream");
    }
}