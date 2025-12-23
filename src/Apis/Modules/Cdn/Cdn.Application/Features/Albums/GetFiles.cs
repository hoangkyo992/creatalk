using Cdn.Application.Shared.Configurations;
using HungHd.LinqExtensions;
using Microsoft.Extensions.Options;

namespace Cdn.Application.Features.Albums;

public class GetFiles
{
    public record Request : DataSourceRequest, IRequest<ApiResult<Result>>
    {
        [JsonIgnore]
        public long Id { get; set; }
    }

    public record Result : DataSourceResult<Result.ItemDto>
    {
        public class ItemDto : BaseDto
        {
            [JsonConverter(typeof(ZCodeJsonConverter))]
            public long FileId { get; set; }

            public string Title { get; init; }
            public string Description { get; init; }
            public int Index { get; init; }
            public string Name { get; init; }
            public int Size { get; init; }
            public int StatusId { get; init; }
            public string? Url { get; set; }
            public FileType FileTypeId { get; set; }
            public string? Extension { get; init; }
            public string? MineType { get; set; }
            public string? Properties { get; init; }
        }

        public Result(DataSourceResult<ItemDto> result) : base(result)
        {
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
            var data = await appContext.AlbumFiles
                .Where(c => c.AlbumId == request.Id)
                .OrderBy(c => c.Index)
                .Select(c => new Result.ItemDto
                {
                    Id = c.Id,
                    FileId = c.FileId,
                    Index = c.Index,
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
                }).ToDataSourceResultAsync(request, cancellationToken);

            var extensions = await appContext.FileExtensions
                .Where(c => !c.IsDisabled)
                .AsNoTracking()
                .ToListAsync(cancellationToken);

            data.Data.ForEach(v =>
            {
                if (!string.IsNullOrWhiteSpace(v.Url))
                    v.Url = $"{options.Value.PublicPath}/{v.Url}";
                var ext = extensions.Find(x => x.Name.Equals(v.Extension, StringComparison.OrdinalIgnoreCase));
                v.MineType = ext?.MineType;
                v.FileTypeId = ext?.TypeId ?? FileType.None;
            });

            return new SuccessResult<Result>(new Result(data));
        }
    }
}