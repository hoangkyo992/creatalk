using Cdn.Application.Shared.Configurations;
using Microsoft.Extensions.Options;

namespace Cdn.Application.Features.Public;

public class GetAlbumItems
{
    public record Request : IRequest<ApiResult<Result>>
    {
        public string Album { get; set; }
    }

    public class Result : BaseDto
    {
        public string Name { get; init; }
        public int NumberOfItems { get; init; }
        public string Description { get; init; }

        public List<FileDto> Files { get; set; } = [];

        public class FileDto : BaseDto
        {
            [JsonConverter(typeof(ZCodeJsonConverter))]
            public long FileId { get; set; }

            public string Title { get; init; }
            public string Description { get; init; }
            public string Name { get; init; }
            public int Size { get; init; }
            public int StatusId { get; init; }
            public string? Url { get; set; }
            public FileType FileTypeId { get; set; }
            public string? Extension { get; init; }
            public string? MineType { get; set; }
            public string? Properties { get; init; }
        }
    }

    public class Validator : AbstractValidator<Request>
    {
        public Validator()
        {
            RuleFor(v => v.Album)
                .NotNull()
                .NotEmpty();
        }
    }

    public class Handler(IAppContext appContext, IOptions<CdnServerConfiguration> options) : IRequestHandler<Request, ApiResult<Result>>
    {
        public async Task<ApiResult<Result>> Handle(Request request, CancellationToken cancellationToken)
        {
            var item = await appContext.Albums
                .Where(c => c.Name.ToLower() == request.Album.ToLower())
                .Select(c => new Result
                {
                    Id = c.Id,
                    Name = c.Name,
                    Description = c.Description,
                    NumberOfItems = c.Files.Count,
                    CreatedBy = c.CreatedBy,
                    CreatedTime = c.CreatedTime,
                    UpdatedBy = c.UpdatedBy,
                    UpdatedTime = c.UpdatedTime,
                }).FirstOrDefaultAsync(cancellationToken);

            if (item == null)
                return new FailResult<Result>(ErrorMessages.ALBUM_NOT_FOUND, HttpStatusCode.NotFound);

            item.Files = await appContext.AlbumFiles
                .Where(c => c.AlbumId == item.Id)
                .OrderBy(c => c.Index)
                .Select(c => new Result.FileDto
                {
                    Id = c.Id,
                    FileId = c.FileId,
                    Description = c.Description,
                    Title = c.Title,
                    Name = c.File.Name,
                    Size = c.File.Size,
                    Url = c.File.Url,
                    StatusId = (int)c.File.StatusId,
                    Extension = c.File.Extension,
                    FileTypeId = c.File.TypeId,
                    Properties = c.File.Properties,
                    CreatedBy = c.CreatedBy,
                    CreatedTime = c.CreatedTime,
                    UpdatedBy = c.UpdatedBy,
                    UpdatedTime = c.UpdatedTime
                }).ToListAsync(cancellationToken);

            var extensions = await appContext.FileExtensions
                .Where(c => !c.IsDisabled)
                .AsNoTracking()
                .ToListAsync(cancellationToken);

            item.Files.ForEach(v =>
            {
                if (!string.IsNullOrWhiteSpace(v.Url))
                    v.Url = $"{options.Value.PublicPath}/{v.Url}";
                var ext = extensions.Find(x => x.Name.Equals(v.Extension, StringComparison.OrdinalIgnoreCase));
                v.MineType = ext?.MineType;
                v.FileTypeId = ext?.TypeId ?? FileType.None;
            });

            return new SuccessResult<Result>(item);
        }
    }
}