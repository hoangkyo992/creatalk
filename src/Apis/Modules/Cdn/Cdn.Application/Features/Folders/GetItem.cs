using Cdn.Application.Shared.Configurations;
using Microsoft.Extensions.Options;

namespace Cdn.Application.Features.Folders;

public class GetItem
{
    public record Request : IRequest<ApiResult<Result>>
    {
        [JsonIgnore]
        public long Id { get; set; }

        public bool IncludeArchivedItems { get; init; }

        public FileType? TypeId { get; init; }
    }

    public class Result : BaseDto
    {
        public string Name { get; init; }
        public FolderStatus StatusId { get; init; }
        public string StatusCode => EnumHelper<FolderStatus>.GetLocalizedKey(StatusId);

        [JsonConverter(typeof(ZCodeJsonConverter))]
        public long? ParentId { get; init; }

        public List<Item> Items { get; set; }

        public class Item : BaseDto
        {
            public string Name { get; init; }
            public int Size { get; init; }
            public bool IsDirectory { get; init; }
            public int StatusId { get; init; }
            public string? Url { get; set; }
            public FileType FileTypeId { get; set; }
            public string? Extension { get; init; }
            public string? MineType { get; set; }
            public string? Properties { get; init; }

            [JsonConverter(typeof(ZCodeJsonConverter))]
            public long? ParentId { get; init; }
        }
    }

    public class Validator : AbstractValidator<Request>
    {
        public Validator()
        {
            RuleFor(v => v.Id)
                .NotNull()
                .GreaterThan(0);
        }
    }

    public class Handler(IAppContext appContext, IOptions<CdnServerConfiguration> options) : IRequestHandler<Request, ApiResult<Result>>
    {
        public async Task<ApiResult<Result>> Handle(Request request, CancellationToken cancellationToken)
        {
            var item = await appContext.Folders
                .Where(c => c.Id == request.Id)
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
                    ParentId = c.ParentId
                }).FirstOrDefaultAsync(cancellationToken);

            if (item == null)
                return new FailResult<Result>(ErrorMessages.FOLDER_NOT_FOUND, HttpStatusCode.NotFound);

            var statusIds = new List<FolderStatus> { FolderStatus.Active };
            if (request.IncludeArchivedItems)
                statusIds.Add(FolderStatus.Archived);

            item.Items = await appContext.Folders
                .Where(c => c.ParentId == request.Id)
                .Where(c => statusIds.Contains(c.StatusId))
                .Select(c => new Result.Item
                {
                    Id = c.Id,
                    Name = c.Name,
                    ParentId = c.ParentId,
                    StatusId = (int)c.StatusId,
                    IsDirectory = true,
                    CreatedBy = c.CreatedBy,
                    CreatedTime = c.CreatedTime,
                    UpdatedBy = c.UpdatedBy,
                    UpdatedTime = c.UpdatedTime
                }).ToListAsync(cancellationToken);

            var fileStatusIds = new List<FileStatus> { FileStatus.Active };
            if (request.IncludeArchivedItems)
                fileStatusIds.Add(FileStatus.Archived);

            var qFiles = appContext.Files
                .Where(c => c.FolderId == request.Id)
                .Where(c => fileStatusIds.Contains(c.StatusId));

            if (request.TypeId.HasValue)
            {
                var exts = await appContext.FileExtensions
                    .Where(c => !c.IsDisabled)
                    .Where(c => c.TypeId == request.TypeId)
                    .Select(c => c.Name.ToLower())
                    .ToListAsync(cancellationToken);
                qFiles = qFiles.Where(c => exts.Contains(c.Extension));
            }

            var files = await qFiles
                .Select(c => new Result.Item
                {
                    Id = c.Id,
                    Name = c.Name,
                    Size = c.Size,
                    Url = c.Url,
                    StatusId = (int)c.StatusId,
                    IsDirectory = false,
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
            item.Items.AddRange(files);

            return new SuccessResult<Result>(item);
        }
    }
}