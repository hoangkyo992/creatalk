namespace Cdn.Application.Features.Albums;

public class RemoveFiles
{
    public record Command : IRequest<ApiResult<Result>>
    {
        [JsonIgnore]
        public long Id { get; set; }

        [JsonConverter(typeof(ZCodeCollectionJsonConverter<IEnumerable<long>>))]
        public IEnumerable<long> FileIds { get; init; } = [];
    }

    public class Result : BaseDto
    {
    }

    public class Validator : AbstractValidator<Command>
    {
        public Validator()
        {
            RuleFor(v => v.Id)
                .NotNull()
                .GreaterThan(0);
            RuleFor(v => v.FileIds)
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
            var item = await appContext.Albums
                .Where(c => c.Id == command.Id)
                .Include(c => c.Files)
                .FirstOrDefaultAsync(cancellationToken);
            if (item == null)
                return new FailResult<Result>(ErrorMessages.ALBUM_NOT_FOUND, HttpStatusCode.NotFound);

            foreach (var fileId in command.FileIds)
            {
                var file = item.Files.FirstOrDefault(x => x.Id == fileId);
                if (file == null)
                    continue;

                file.IsDeleted = true;
            }

            await appContext.SaveChangesAsync(cancellationToken);
            await mediator.Publish(new OnAlbumFilesRemovedEvent
            {
                Id = item.Id,
                FileIds = command.FileIds.ToList()
            }.SetCurrentUser(currentUser), cancellationToken);

            return new SuccessResult<Result>(new Result
            {
                Id = item.Id,
                CreatedBy = item.CreatedBy,
                CreatedTime = item.CreatedTime,
            });
        }
    }
}