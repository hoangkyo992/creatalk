namespace Cdn.Application.Features.Albums;

public class UpdateFile
{
    public record Command : IRequest<ApiResult<Result>>
    {
        [JsonIgnore]
        public long AlbumId { get; set; }

        [JsonIgnore]
        public long FileId { get; set; }

        public string Title { get; init; }
        public string Description { get; init; }
    }

    public class Result : BaseDto
    {
        public string Title { get; init; }
        public string Description { get; init; }
    }

    public class Validator : AbstractValidator<Command>
    {
        public Validator()
        {
            RuleFor(v => v.AlbumId)
                .NotNull()
                .GreaterThan(0);
            RuleFor(v => v.FileId)
                .NotNull()
                .GreaterThan(0);
        }
    }

    public class Handler(IAppContext appContext) : IRequestHandler<Command, ApiResult<Result>>
    {
        public async Task<ApiResult<Result>> Handle(Command command, CancellationToken cancellationToken)
        {
            var item = await appContext.Albums
                .Where(c => c.Id == command.AlbumId)
                .Include(c => c.Files)
                .FirstOrDefaultAsync(cancellationToken);
            if (item == null)
                return new FailResult<Result>(ErrorMessages.ALBUM_NOT_FOUND, HttpStatusCode.NotFound);

            var file = item.Files.FirstOrDefault(c => c.Id == command.FileId);
            if (file == null)
                return new FailResult<Result>(ErrorMessages.FILE_NOT_FOUND, HttpStatusCode.NotFound);

            file.Title = command.Title;
            file.Description = command.Description;

            await appContext.SaveChangesAsync(cancellationToken);

            return new SuccessResult<Result>(new Result
            {
                Id = file.Id,
                CreatedBy = file.CreatedBy,
                CreatedTime = file.CreatedTime,
                Title = file.Title,
                Description = file.Description
            });
        }
    }
}