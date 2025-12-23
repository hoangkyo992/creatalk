using Cdn.Domain.Shared.Constants;

namespace Cdn.Application.Features.Folders;

public class Move
{
    public record Command : IRequest<ApiResult<Result>>
    {
        [JsonIgnore]
        public long Id { get; set; }

        [JsonConverter(typeof(ZCodeJsonConverter))]
        public long ParentId { get; init; }
    }

    public record Result
    {
        [JsonConverter(typeof(ZCodeJsonConverter))]
        public long Id { get; init; }
    }

    public class Validator : AbstractValidator<Command>
    {
        public Validator()
        {
            RuleFor(v => v.Id)
                .NotNull()
                .GreaterThan(0);

            RuleFor(v => v.ParentId)
                .NotNull()
                .GreaterThan(0)
                .NotEqual(v => v.Id);
        }
    }

    public class Handler(IMediator mediator,
        IAppContext appContext,
        ICurrentUser currentUser) : IRequestHandler<Command, ApiResult<Result>>
    {
        public async Task<ApiResult<Result>> Handle(Command command, CancellationToken cancellationToken)
        {
            var item = await appContext.Folders
                  .Where(c => c.Id == command.Id)
                  .Where(c => c.Name != FolderConstants.RootFolderName)
                  .FirstOrDefaultAsync(cancellationToken);
            if (item == null)
                return new FailResult<Result>(ErrorMessages.FOLDER_NOT_FOUND, HttpStatusCode.NotFound);

            var parent = await appContext.Folders
                 .Where(c => c.Id == command.ParentId)
                 .FirstOrDefaultAsync(cancellationToken);
            if (parent == null)
                return new FailResult<Result>(ErrorMessages.FOLDER_NOT_FOUND, "parentId", command.ParentId, HttpStatusCode.NotFound);

            if (parent.Id != item.ParentId)
            {
                var existingItem = await appContext.Folders
                    .Where(c => c.Id != command.Id && c.ParentId == command.ParentId && c.Name.ToLower() == item.Name.ToLower())
                    .Where(c => c.StatusId != FolderStatus.IsInTrash)
                    .AsNoTracking()
                    .FirstOrDefaultAsync(cancellationToken);
                if (existingItem != null)
                    return new FailResult<Result>(ErrorMessages.FOLDER_NAME_EXISTED, "name", existingItem.Name, HttpStatusCode.Conflict);

                var childrenIds = await GetAllChildrenIds([item.Id], cancellationToken);
                if (childrenIds.Contains(command.ParentId))
                {
                    return new FailResult<Result>(ErrorMessages.FOLDER_PARENT_INVALID, "parentId", command.ParentId, HttpStatusCode.NotAcceptable);
                }

                var evt = new OnFolderUpdatedEvent { Ids = [command.ParentId] };
                if (item.ParentId > 0) evt.Ids.Add(item.ParentId.Value);

                item.ParentId = parent.Id;

                await appContext.SaveChangesAsync(cancellationToken);

                await mediator.Publish(new OnFolderMovedEvent { Ids = [command.Id] }.SetCurrentUser(currentUser), cancellationToken);
                await mediator.Publish(evt.SetCurrentUser(currentUser), cancellationToken);
            }

            return new SuccessResult<Result>(new Result
            {
                Id = item.Id
            });
        }

        private async Task<List<long>> GetAllChildrenIds(List<long> pIds, CancellationToken cancellationToken)
        {
            var data = await appContext.Folders
                .Where(c => c.ParentId.HasValue && pIds.Contains(c.ParentId.Value))
                .ToListAsync(cancellationToken);
            if (data.Count != 0)
            {
                var ids = data.Select(c => c.Id).ToList();
                var childIds = await GetAllChildrenIds(ids, cancellationToken);
                return [.. ids, .. childIds];
            }
            return [];
        }
    }
}