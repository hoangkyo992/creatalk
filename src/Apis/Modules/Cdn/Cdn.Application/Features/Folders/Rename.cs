namespace Cdn.Application.Features.Folders;

public class Rename
{
    public record Command : IRequest<ApiResult<Result>>
    {
        [JsonIgnore]
        public long Id { get; set; }

        public string Name { get; init; }
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
                  .Where(c => c.Id == command.Id)
                  .Where(c => c.StatusId != FolderStatus.IsInTrash)
                  .FirstOrDefaultAsync(cancellationToken);
            if (item == null)
                return new FailResult<Result>(ErrorMessages.FOLDER_NOT_FOUND, HttpStatusCode.NotFound);

            if (item.Name.Equals(command.Name, StringComparison.OrdinalIgnoreCase))
            {
                return new SuccessResult<Result>(new Result
                {
                    Id = item.Id
                });
            }

            var existedFolderName = await appContext.Folders
                .AsNoTracking()
                .Where(c => c.ParentId == item.ParentId && c.Name.ToLower() == command.Name.ToLower())
                .FirstOrDefaultAsync(cancellationToken);
            if (existedFolderName != null)
            {
                return new FailResult<Result>(ErrorMessages.FOLDER_NAME_EXISTED, "name", existedFolderName.Name, HttpStatusCode.NotAcceptable);
            }

            item.Name = command.Name;

            await appContext.SaveChangesAsync(cancellationToken);

            await mediator.Publish(new OnFolderRenamedEvent { Ids = [command.Id] }.SetCurrentUser(currentUser), cancellationToken);

            return new SuccessResult<Result>(new Result
            {
                Id = item.Id
            });
        }
    }
}