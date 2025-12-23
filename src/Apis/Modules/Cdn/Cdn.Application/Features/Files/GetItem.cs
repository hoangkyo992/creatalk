using Cdn.Application.Shared.Configurations;
using Microsoft.Extensions.Options;

namespace Cdn.Application.Features.Files;

public class GetItem
{
    public record Request : IRequest<ApiResult<Result>>
    {
        [JsonIgnore]
        public long Id { get; set; }
    }

    public class Result : BaseDto
    {
        public string Name { get; init; }

        public string Url { get; init; }

        public string PublicUrl { get; set; }

        [JsonConverter(typeof(ZCodeJsonConverter))]
        public long FolderId { get; init; }

        public string FolderName { get; init; }

        public byte[] Content { get; init; }

        public int Size { get; init; }

        public FileType FileTypeId { get; set; }

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
            RuleFor(v => v.Id)
                .NotNull()
                .GreaterThan(0);
        }
    }

    public class Handler(IAppContext appContext, IOptions<CdnServerConfiguration> options) : IRequestHandler<Request, ApiResult<Result>>
    {
        public async Task<ApiResult<Result>> Handle(Request request, CancellationToken cancellationToken)
        {
            var item = await appContext.Files
                .Where(c => c.Id == request.Id)
                .Select(c => new Result
                {
                    Id = c.Id,
                    CreatedBy = c.CreatedBy,
                    CreatedTime = c.CreatedTime,
                    UpdatedBy = c.UpdatedBy,
                    UpdatedTime = c.UpdatedTime,
                    Name = c.Name,
                    Url = c.Url,
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

            var ext = await appContext.FileExtensions
                .Where(c => !c.IsDisabled)
                .AsNoTracking()
                .Where(x => x.Name.ToLower() == item.Extension.ToLower())
                .FirstOrDefaultAsync(cancellationToken);

            var extension = await appContext.FileExtensions.Where(c => c.IsDisabled && c.Name == item.Extension).FirstOrDefaultAsync(cancellationToken);
            item.MineType = extension?.MineType;
            item.FileTypeId = ext?.TypeId ?? FileType.None;

            if (!string.IsNullOrWhiteSpace(item.Url))
                item.PublicUrl = $"{options.Value.PublicPath}/{item.Url}";

            return new SuccessResult<Result>(item);
        }
    }
}