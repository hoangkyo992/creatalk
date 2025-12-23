namespace Cdn.Application.Features.Files;

public class Delete
{
    public record Command : IRequest<ApiResult<Result>>
    {
        [JsonConverter(typeof(ZCodeJsonConverter))]
        public long Id { get; init; }
    }

    public record Result
    {
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
            var item = await appContext.Files
                .Where(c => c.Id == command.Id)
                .Where(c => c.StatusId == FileStatus.IsInTrash)
                .FirstOrDefaultAsync(cancellationToken);
            if (item == null)
                return new FailResult<Result>(ErrorMessages.FILE_NOT_FOUND, HttpStatusCode.NotFound);

            item.IsDeleted = true;

            await appContext.SaveChangesAsync(cancellationToken);
            await mediator.Publish(new OnFileDeletedEvent
            {
                FileIds = [item.Id],
                FolderId = item.FolderId
            }.SetCurrentUser(currentUser), cancellationToken);

            return new SuccessResult<Result>(new Result());
        }
    }
}