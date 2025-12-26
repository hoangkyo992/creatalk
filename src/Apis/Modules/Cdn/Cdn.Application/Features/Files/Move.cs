namespace Cdn.Application.Features.Files;

public class Move
{
    public record Command : IRequest<ApiResult<Result>>
    {
        [JsonIgnore]
        public long Id { get; set; }

        [JsonConverter(typeof(ZCodeJsonConverter))]
        public long FolderId { get; init; }
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

            RuleFor(v => v.FolderId)
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
            var item = await appContext.Files
                  .Where(c => c.Id == command.Id)
                  .Where(c => c.StatusId != FileStatus.IsInTrash)
                  .FirstOrDefaultAsync(cancellationToken);
            if (item == null)
                return new FailResult<Result>(ErrorMessages.FILE_NOT_FOUND, HttpStatusCode.NotFound);

            if (item.FolderId == command.FolderId)
            {
                return new SuccessResult<Result>(new Result
                {
                    Id = item.Id
                });
            }

            var folder = await appContext.Folders
                 .Where(c => c.Id == command.FolderId)
                 .FirstOrDefaultAsync(cancellationToken);
            if (folder == null)
                return new FailResult<Result>(ErrorMessages.FOLDER_NOT_FOUND, HttpStatusCode.NotFound);

            var existedFileName = await appContext.Files
                .AsNoTracking()
                .Where(c => c.FolderId == folder.Id && c.Name.ToLower() == item.Name.ToLower())
                .Where(c => c.StatusId != FileStatus.IsInTrash)
                .FirstOrDefaultAsync(cancellationToken);
            if (existedFileName != null)
            {
                return new FailResult<Result>(ErrorMessages.FILE_NAME_EXISTED, "name", existedFileName.Name, HttpStatusCode.NotAcceptable);
            }

            item.FolderId = folder.Id;

            await appContext.SaveChangesAsync(cancellationToken);

            await mediator.Publish(new OnFileMovedEvent { FileIds = [command.Id], FolderId = command.FolderId, FolderName = folder.Name }.SetCurrentUser(currentUser), cancellationToken);

            return new SuccessResult<Result>(new Result
            {
                Id = item.Id
            });
        }
    }
}