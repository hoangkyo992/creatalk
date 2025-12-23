using Cdn.Application.Shared.Configurations;
using Microsoft.Extensions.Options;

namespace Cdn.Application.Features.Public;

public class GetFolderItems
{
    public record Request : IRequest<ApiResult<Result>>
    {
        [JsonConverter(typeof(ZCodeJsonConverter))]
        public long FolderId { get; set; }
    }

    public class Result : BaseDto
    {
        public string Name { get; init; }
        public FolderStatus StatusId { get; init; }
        public string StatusCode => EnumHelper<FolderStatus>.GetLocalizedKey(StatusId);

        public List<FileDto> Files { get; set; } = [];

        public class FileDto : BaseDto
        {
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
            RuleFor(v => v.FolderId)
                .NotNull()
                .GreaterThan(0);
        }
    }

    public class Handler(IAppContext appContext, IOptions<CdnServerConfiguration> options) : IRequestHandler<Request, ApiResult<Result>>
    {
        public async Task<ApiResult<Result>> Handle(Request request, CancellationToken cancellationToken)
        {
            var item = await appContext.Folders
                .Where(c => c.Id == request.FolderId)
                .Where(c => c.StatusId != FolderStatus.IsInTrash)
                .Select(c => new Result
                {
                    Id = c.Id,
                    CreatedBy = c.CreatedBy,
                    CreatedTime = c.CreatedTime,
                    StatusId = c.StatusId,
                    UpdatedBy = c.UpdatedBy,
                    UpdatedTime = c.UpdatedTime,
                    Name = c.Name,
                }).FirstOrDefaultAsync(cancellationToken);

            if (item == null)
                return new FailResult<Result>(ErrorMessages.FOLDER_NOT_FOUND, HttpStatusCode.NotFound);

            var fileStatusIds = new List<FileStatus> { FileStatus.Active };
            var files = await appContext.Files
                .Where(c => c.FolderId == request.FolderId)
                .Where(c => fileStatusIds.Contains(c.StatusId))
                .Select(c => new Result.FileDto
                {
                    Id = c.Id,
                    Name = c.Name,
                    Size = c.Size,
                    Url = c.Url,
                    StatusId = (int)c.StatusId,
                    Extension = c.Extension,
                    FileTypeId = c.TypeId,
                    Properties = c.Properties,
                    CreatedBy = c.CreatedBy,
                    CreatedTime = c.CreatedTime,
                    UpdatedBy = c.UpdatedBy,
                    UpdatedTime = c.UpdatedTime
                }).ToListAsync(cancellationToken);

            var extensions = await appContext.FileExtensions
                .Where(c => !c.IsDisabled)
                .AsNoTracking()
                .ToListAsync(cancellationToken);

            files.ForEach(v =>
            {
                if (!string.IsNullOrWhiteSpace(v.Url))
                    v.Url = $"{options.Value.PublicPath}/{v.Url}";
                var ext = extensions.Find(x => x.Name.Equals(v.Extension, StringComparison.OrdinalIgnoreCase));
                v.MineType = ext?.MineType;
                v.FileTypeId = ext?.TypeId ?? FileType.None;
            });
            item.Files.AddRange(files);

            return new SuccessResult<Result>(item);
        }
    }
}