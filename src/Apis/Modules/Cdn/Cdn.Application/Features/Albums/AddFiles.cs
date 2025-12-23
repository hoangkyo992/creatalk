namespace Cdn.Application.Features.Albums;

public class AddFiles
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
        public int NumberOfItems { get; init; }
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

            List<AlbumFile> files = [];
            var index = item.Files.Select(c => c.Index).DefaultIfEmpty(0).Max() + 1;
            foreach (var fileId in command.FileIds)
            {
                if (item.Files.Any(x => x.FileId == fileId))
                    continue;
                files.Add(new AlbumFile
                {
                    Index = index++,
                    FileId = fileId,
                    AlbumId = item.Id
                });
            }

            appContext.AlbumFiles.AddRange(files);
            await appContext.SaveChangesAsync(cancellationToken);
            await mediator.Publish(new OnAlbumFilesAddedEvent
            {
                Id = item.Id,
                FileIds = [.. files.Select(c => c.Id)]
            }.SetCurrentUser(currentUser), cancellationToken);

            return new SuccessResult<Result>(new Result
            {
                Id = item.Id,
                CreatedBy = item.CreatedBy,
                CreatedTime = item.CreatedTime,
                NumberOfItems = files.Count
            });
        }
    }
}