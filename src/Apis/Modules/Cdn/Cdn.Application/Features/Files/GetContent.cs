using Auth.Application.Shared;
using Auth.Application.Shared.Settings;
using ImageSize = Cdn.Domain.Shared.Enums.ImageSize;

namespace Cdn.Application.Features.Files;

public class GetContent
{
    public record Request : IRequest<ApiResult<Result>>
    {
        public string Path { get; set; }
        public long Id { get; set; }
        public ImageSize Size { get; set; } = ImageSize.Original;
    }

    public class Result : BaseDto
    {
        public string Name { get; init; }

        [JsonConverter(typeof(ZCodeJsonConverter))]
        public long FolderId { get; init; }

        public string FolderName { get; init; }

        public byte[] Content { get; set; }

        public int Size { get; set; }

        public string Extension { get; init; }

        public string Properties { get; init; }

        public string? MineType { get; set; }

        public FileStatus StatusId { get; init; }

        public string StatusCode => EnumHelper<FileStatus>.GetLocalizedKey(StatusId);
    }

    public class Validator : AbstractValidator<Request>
    {
        public Validator()
        {
        }
    }

    public class Handler(IAppContext appContext, ISettingService settingService, IMemoryCache cache) : IRequestHandler<Request, ApiResult<Result>>
    {
        public async Task<ApiResult<Result>> Handle(Request request, CancellationToken cancellationToken)
        {
            var q = appContext.Files
                .IgnoreQueryFilters()
                .Where(c => !c.Folder.IsDeleted);
            if (request.Id > 0)
            {
                q = q.Where(c => c.Id == request.Id);
            }
            else
            {
                string fileName = string.Empty;
                string folderPath = string.Empty;
                try
                {
                    fileName = Path.GetFileName(request.Path);
                    folderPath = request.Path[..^fileName.Length];
                }
                catch
                {
                    return new FailResult<Result>(ErrorMessages.FILE_NOT_FOUND, HttpStatusCode.NotFound);
                }

                q = q.Where(c => c.Url.ToLower() == fileName.ToLower());
            }

            var item = await q
                .Select(c => new Result
                {
                    Id = c.Id,
                    CreatedBy = c.CreatedBy,
                    CreatedTime = c.CreatedTime,
                    UpdatedBy = c.UpdatedBy,
                    UpdatedTime = c.UpdatedTime,
                    Name = c.Name,
                    Size = c.Size,
                    Properties = c.Properties,
                    StatusId = c.StatusId,
                    Content = c.Content,
                    FolderId = c.FolderId,
                    FolderName = c.Folder.Name,
                    Extension = c.Extension,
                }).FirstOrDefaultAsync(cancellationToken);

            if (item == null)
                return new FailResult<Result>(ErrorMessages.FILE_NOT_FOUND, HttpStatusCode.NotFound);

            var extension = await appContext.FileExtensions.Where(c => !c.IsDisabled && c.Name == item.Extension).FirstOrDefaultAsync(cancellationToken);
            item.MineType = extension?.MineType;

            if (request.Size != ImageSize.Original)
            {
                item.Content = await ResizeWithSettingAsync(item.Content, request.Size, cancellationToken);
                item.Size = item.Content.Length;
            }

            return new SuccessResult<Result>(item);
        }

        private async Task<byte[]> ResizeWithSettingAsync(byte[] content, ImageSize size, CancellationToken cancellationToken = default)
        {
            var cacheKey = $"{Assembly.Name}-{nameof(GetContent)}-{nameof(ResizeWithSettingAsync)}-GetSettings-{SettingKeys.Media}";
            if (!cache.TryGetValue(cacheKey, out MediaSetting? setting))
            {
                setting = await settingService.GetAsync<MediaSetting>(SettingKeys.Media, cancellationToken);
                var cacheOptions = new MemoryCacheEntryOptions()
                    .SetSlidingExpiration(TimeSpan.FromSeconds(30))
                    .SetAbsoluteExpiration(TimeSpan.FromSeconds(600))
                    .SetPriority(CacheItemPriority.Normal);
                cache.Set(cacheKey, setting, cacheOptions);
            }

            var imageSize = size == ImageSize.Thumb
                ? setting!.ImageSetting.ThumbnailImageSize
                : size == ImageSize.Medium
                    ? setting!.ImageSetting.MediumImageSize
                    : setting!.ImageSetting.LargeImageSize;

            return ImageResizer.Resize(content, imageSize.MaxWidth, imageSize.MaxHeight);
        }
    }
}