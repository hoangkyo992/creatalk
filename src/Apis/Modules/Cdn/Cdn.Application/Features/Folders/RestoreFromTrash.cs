using Cdn.Domain.Shared.Constants;

namespace Cdn.Application.Features.Folders;

public class RestoreFromTrash
{
    public record Command : IRequest<ApiResult<Result>>
    {
        [JsonIgnore]
        public long Id { get; set; }
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
                  .Where(c => c.StatusId == FolderStatus.IsInTrash)
                  .FirstOrDefaultAsync(cancellationToken);
            if (item == null)
                return new FailResult<Result>(ErrorMessages.FOLDER_NOT_FOUND, HttpStatusCode.NotFound);

            var existingItem = await appContext.Folders
                .Where(c => c.Id != command.Id && c.ParentId == item.ParentId && c.Name.ToLower() == item.Name.ToLower())
                .Where(c => c.StatusId != FolderStatus.IsInTrash)
                .AsNoTracking()
                .FirstOrDefaultAsync(cancellationToken);
            if (existingItem != null)
                return new FailResult<Result>(ErrorMessages.FOLDER_NAME_EXISTED, "name", item.Name, HttpStatusCode.Conflict);

            item.StatusId = FolderStatus.Active;

            await appContext.SaveChangesAsync(cancellationToken);

            await mediator.Publish(new OnFolderRestoreFromTrashEvent { Ids = [command.Id] }.SetCurrentUser(currentUser), cancellationToken);

            return new SuccessResult<Result>(new Result
            {
                Id = item.Id
            });
        }
    }
}