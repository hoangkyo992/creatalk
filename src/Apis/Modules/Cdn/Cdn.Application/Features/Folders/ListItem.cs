using Cdn.Domain.Shared.Constants;
using HungHd.LinqExtensions;

namespace Cdn.Application.Features.Folders;

public class ListItem
{
    public record Request : IRequest<ApiResult<Result>>
    {
        [JsonConverter(typeof(ZCodeJsonConverter))]
        public long? ParentId { get; init; }
    }

    public record Result
    {
        public List<Item> Items { get; init; }

        public class Item : BaseDto
        {
            public string Name { get; init; }
            public FolderStatus StatusId { get; init; }
            public string StatusCode => EnumHelper<FolderStatus>.GetLocalizedKey(StatusId);

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

    public class Handler(IAppContext appContext) : IRequestHandler<Request, ApiResult<Result>>
    {
        public async Task<ApiResult<Result>> Handle(Request request, CancellationToken cancellationToken)
        {
            var data = await appContext.Folders
                .WhereIf(request.ParentId > 0, c => c.ParentId == request.ParentId)
                .Where(c => c.StatusId != FolderStatus.IsInTrash)
                .SelectWithDefaultOrder(c => new Result.Item
                {
                    Id = c.Id,
                    Name = c.Name,
                    ParentId = c.ParentId,
                    StatusId = c.StatusId,
                    CreatedBy = c.CreatedBy,
                    CreatedTime = c.CreatedTime,
                    UpdatedBy = c.UpdatedBy,
                    UpdatedTime = c.UpdatedTime
                }).ToListAsync(cancellationToken);

            if (request.ParentId == null && data.Count == 0)
            {
                data = [await CreateRoot(cancellationToken)];
            }

            return new SuccessResult<Result>(new Result { Items = data });
        }

        private async Task<Result.Item> CreateRoot(CancellationToken cancellationToken)
        {
            var item = new Folder
            {
                Name = FolderConstants.RootFolderName
            };

            appContext.Folders.Add(item);
            await appContext.SaveChangesAsync(cancellationToken);

            return new Result.Item
            {
                Id = item.Id,
                Name = item.Name,
                StatusId = FolderStatus.Active,
                ParentId = item.ParentId,
                CreatedBy = item.CreatedBy,
                CreatedTime = item.CreatedTime,
                UpdatedBy = item.UpdatedBy,
                UpdatedTime = item.UpdatedTime
            };
        }
    }
}