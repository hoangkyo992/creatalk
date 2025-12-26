using Cdn.Application.Shared.Configurations;
using HungHd.LinqExtensions;
using Microsoft.Extensions.Options;

namespace Cdn.Application.Features.Files;

public class ListItem
{
    public record Request : DataSourceRequest, IRequest<ApiResult<Result>>
    {
        [JsonConverter(typeof(ZCodeJsonConverter))]
        public long? FolderId { get; init; }
    }

    public record Result : DataSourceResult<Result.Item>
    {
        public Result(DataSourceResult<Item> result) : base(result)
        {
        }

        public class Item : BaseDto
        {
            public string Name { get; init; }

            public string Url { get; init; }

            public string PublicUrl { get; set; }

            [JsonConverter(typeof(ZCodeJsonConverter))]
            public long FolderId { get; init; }

            public string FolderName { get; init; }

            public int Size { get; init; }

            public FileType FileTypeId { get; set; }

            public string Extension { get; init; }

            public string Properties { get; init; }

            public string? MineType { get; set; }

            public FileStatus StatusId { get; init; }

            public string StatusCode => EnumHelper<FileStatus>.GetLocalizedKey(StatusId);
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
            var data = await appContext.Files
                .WhereIf(request.FolderId.HasValue, c => c.FolderId == request.FolderId)
                .Where(c => c.StatusId == FileStatus.Active)
                .SelectWithDefaultOrder(c => new Result.Item
                {
                    Id = c.Id,
                    Name = c.Name,
                    Size = c.Size,
                    Url = c.Url,
                    StatusId = c.StatusId,
                    FolderId = c.FolderId,
                    FolderName = c.Folder.Name,
                    Extension = c.Extension,
                    FileTypeId = c.TypeId,
                    Properties = c.Properties,
                    CreatedBy = c.CreatedBy,
                    CreatedTime = c.CreatedTime,
                    UpdatedBy = c.UpdatedBy,
                    UpdatedTime = c.UpdatedTime
                }).ToDataSourceResultAsync(request, cancellationToken);

            var extensions = await appContext.FileExtensions
                .Where(c => !c.IsDisabled)
                .AsNoTracking()
                .ToListAsync(cancellationToken);

            data.Data.ForEach(v =>
            {
                if (!string.IsNullOrWhiteSpace(v.Url))
                    v.PublicUrl = $"{options.Value.PublicPath}/{v.Url}";
                var ext = extensions.Find(x => x.Name.Equals(v.Extension, StringComparison.OrdinalIgnoreCase));
                v.MineType = ext?.MineType;
                v.FileTypeId = ext?.TypeId ?? FileType.None;
            });

            return new SuccessResult<Result>(new Result(data));
        }
    }
}