using Auth.Application.Shared;
using Auth.Application.Shared.Settings;
using Cdn.Application.Services;
using ImageSize = Cdn.Domain.Shared.Enums.ImageSize;

namespace Cdn.Application.Features.Files;

public class GetContent
{
    public record Request : IRequest<ApiResult<Result>>
    {
        public string Path { get; set; }
        public long Id { get; set; }
        public ImageSize Size { get; set; } = ImageSize.Original;
        public bool Crop { get; set; }
        public int OffsetX { get; set; }
        public int OffsetY { get; set; }
        public string? ConvertTo { get; set; }
    }

    public class Result : BaseDto
    {
        public string Name { get; init; }

        [JsonConverter(typeof(ZCodeJsonConverter))]
        public long FolderId { get; init; }

        public string FolderName { get; init; }

        public byte[] Content { get; set; }

        public int Size { get; set; }

        public string Properties { get; init; }

        public string Extension { get; set; }

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

    public class Handler(IAppContext appContext,
        ISettingService settingService,
        IImageResizer imageResizer,
        IMemoryCache cache) : IRequestHandler<Request, ApiResult<Result>>
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

            var convertTo = $".{request.ConvertTo}";
            var extensions = await appContext.FileExtensions
                .Where(c => !c.IsDisabled && (c.Name == item.Extension || c.Name == convertTo))
                .ToListAsync(cancellationToken);
            item.MineType = extensions.Find(x => x.Name == item.Extension)?.MineType;

            if (item.MineType == "application/pdf" && (request.ConvertTo == "png" || request.ConvertTo == "jpg"))
            {
                item.Content = imageResizer.ConvertToImage(item.Content);
                item.Size = item.Content.Length;
                item.Extension = convertTo;
                item.MineType = extensions.Find(x => x.Name == item.Extension)?.MineType;
            }

            if (IsImage(item.MineType))
            {
                var setting = await GetMediaSettingAsync(cancellationToken);
                var imageSize = request.Size == ImageSize.Thumb
                    ? setting!.ImageSetting.ThumbnailImageSize
                    : request.Size == ImageSize.Medium
                        ? setting!.ImageSetting.MediumImageSize
                        : setting!.ImageSetting.LargeImageSize;

                if (request.Crop)
                {
                    item.Content = imageResizer.CropImage(item.Content, imageSize.MaxWidth, imageSize.MaxHeight, request.OffsetX, request.OffsetY);
                    item.Size = item.Content.Length;
                }
                else if (request.Size != ImageSize.Original)
                {
                    item.Content = imageResizer.Resize(item.Content, imageSize.MaxWidth, imageSize.MaxHeight);
                    item.Size = item.Content.Length;
                }
            }

            return new SuccessResult<Result>(item);
        }

        private static bool IsImage(string? mineType)
        {
            return !string.IsNullOrWhiteSpace(mineType) && mineType.StartsWith("image/");
        }

        private async Task<MediaSetting> GetMediaSettingAsync(CancellationToken cancellationToken = default)
        {
            var cacheKey = $"{Assembly.Name}-{nameof(GetContent)}-{nameof(GetMediaSettingAsync)}-GetSettings-{SettingKeys.Media}";
            if (!cache.TryGetValue(cacheKey, out MediaSetting? setting))
            {
                setting = await settingService.GetAsync<MediaSetting>(SettingKeys.Media, cancellationToken);
                var cacheOptions = new MemoryCacheEntryOptions()
                    .SetSlidingExpiration(TimeSpan.FromSeconds(30))
                    .SetAbsoluteExpiration(TimeSpan.FromSeconds(600))
                    .SetPriority(CacheItemPriority.Normal);
                cache.Set(cacheKey, setting, cacheOptions);
            }
            return setting!;
        }
    }
}