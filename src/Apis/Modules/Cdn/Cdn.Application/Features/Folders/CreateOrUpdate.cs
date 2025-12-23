namespace Cdn.Application.Features.Folders;

public class CreateOrUpdate
{
    public record Command : IRequest<ApiResult<Result>>
    {
        [JsonIgnore]
        public long Id { get; set; }
        public string Name { get; init; }

        [JsonConverter(typeof(ZCodeJsonConverter))]
        public long? ParentId { get; init; }
    }

    public class Result : BaseDto
    {
        public string Name { get; init; }
        public int Size { get; init; }
        public bool IsDirectory { get; init; }
        public int StatusId { get; init; }
        public string? Url { get; init; }

        [JsonConverter(typeof(ZCodeJsonConverter))]
        public long? ParentId { get; init; }
    }

    public class Validator : AbstractValidator<Command>
    {
        public Validator()
        {
            RuleFor(v => v.Name)
                .NotNull()
                .NotEmpty();
        }
    }

    public class Handler(IMediator mediator,
        IAppContext appContext,
        ICurrentUser currentUser) : IRequestHandler<Command, ApiResult<Result>>
    {
        public async Task<ApiResult<Result>> Handle(Command command, CancellationToken cancellationToken)
        {
            if (command.Name.IsInvalidFolderName())
            {
                return new FailResult<Result>(ErrorMessages.NAME_CONTAINS_INVALID_CHARACTERS, HttpStatusCode.NotAcceptable);
            }

            var item = await appContext.Folders
                .Where(c => c.Id != command.Id && c.ParentId == command.ParentId && c.Name.ToLower() == command.Name.Trim().ToLower())
                .Where(c => c.StatusId != FolderStatus.IsInTrash)
                .FirstOrDefaultAsync(cancellationToken);
            if (item != null)
                return new FailResult<Result>(ErrorMessages.FOLDER_NAME_EXISTED, "name", command.Name, HttpStatusCode.Conflict);

            item = await appContext.Folders
                .Where(c => c.Id == command.Id && c.ParentId == command.ParentId)
                .Where(c => c.StatusId != FolderStatus.IsInTrash)
                .FirstOrDefaultAsync(cancellationToken);
            if (item == null && command.Id > 0)
                return new FailResult<Result>(ErrorMessages.FOLDER_NOT_FOUND, HttpStatusCode.NotFound);

            if (command.ParentId.HasValue)
            {
                var parent = await appContext.Folders
                    .Where(c => c.Id == command.ParentId)
                    .FirstOrDefaultAsync(cancellationToken);
                if (parent == null)
                    return new FailResult<Result>(ErrorMessages.FOLDER_NOT_FOUND, "parentId", command.ParentId, HttpStatusCode.NotFound);
                if (item != null)
                {
                    if (item.Id == command.ParentId)
                        return new FailResult<Result>(ErrorMessages.FOLDER_PARENT_INVALID, "parentId", command.ParentId, HttpStatusCode.NotAcceptable);

                    var childrenIds = await GetAllChildrenIds([item.Id], cancellationToken);
                    if (childrenIds.Contains(command.ParentId.Value))
                    {
                        return new FailResult<Result>(ErrorMessages.FOLDER_PARENT_INVALID, "parentId", command.ParentId, HttpStatusCode.NotAcceptable);
                    }
                }
            }

            ApplicationEvent? evt = null;

            if (item == null)
            {
                item = new Domain.Entities.Folder
                {
                    ParentId = command.ParentId
                };
                appContext.Folders.Add(item);
                evt = new OnFolderCreatedEvent
                {
                    Ids = [item.Id]
                };
            }
            else
            {
                evt = new OnFolderUpdatedEvent
                {
                    Ids = [item.Id]
                };
            }

            item.Name = command.Name.Trim();
            item.ParentId = command.ParentId;

            await appContext.SaveChangesAsync(cancellationToken);
            await mediator.Publish(evt.SetCurrentUser(currentUser), cancellationToken);

            return new SuccessResult<Result>(new Result
            {
                Id = item.Id,
                CreatedBy = item.CreatedBy,
                CreatedTime = item.CreatedTime,
                IsDirectory = true,
                Name = item.Name,
                ParentId = command.ParentId,
                StatusId = (int)item.StatusId,
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