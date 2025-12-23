using Cdn.Application.Shared.Configurations;
using Microsoft.Extensions.Options;

namespace Cdn.Application.Features.Folders;

public class GetItemsInTrash
{
    public record Request : IRequest<ApiResult<Result>>
    {
    }

    public class Result
    {
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
        }
    }

    public class Handler(IAppContext appContext, IOptions<CdnServerConfiguration> options) : IRequestHandler<Request, ApiResult<Result>>
    {
        public async Task<ApiResult<Result>> Handle(Request request, CancellationToken cancellationToken)
        {
            var item = new Result
            {
                Items = await appContext.Folders
                    .Where(c => c.StatusId == FolderStatus.IsInTrash)
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
                    }).ToListAsync(cancellationToken)
            };

            var files = await appContext.Files
                .Where(c => c.StatusId == FileStatus.IsInTrash)
                .Select(c => new Result.Item
                {
                    Id = c.Id,
                    Name = c.Name,
                    Size = c.Size,
                    Url = c.Url,
                    StatusId = (int)c.StatusId,
                    ParentId = c.FolderId,
                    IsDirectory = false,
                    Properties = c.Properties,
                    FileTypeId = c.TypeId,
                    Extension = c.Extension,
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