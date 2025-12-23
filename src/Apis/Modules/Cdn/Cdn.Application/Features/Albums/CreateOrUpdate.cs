namespace Cdn.Application.Features.Albums;

public class CreateOrUpdate
{
    public record Command : IRequest<ApiResult<Result>>
    {
        [JsonIgnore]
        public long Id { get; set; }
        public string Name { get; init; }
        public string Description { get; init; }
    }

    public class Result : BaseDto
    {
        public string Name { get; init; }
    }

    public class Validator : AbstractValidator<Command>
    {
        public Validator()
        {
            RuleFor(v => v.Name)
                .NotNull()
                .NotEmpty();
            RuleFor(v => v.Description)
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
                .Where(c => c.Id != command.Id && c.Name.ToLower() == command.Name.Trim().ToLower())
                .FirstOrDefaultAsync(cancellationToken);
            if (item != null)
                return new FailResult<Result>(ErrorMessages.ALBUM_NAME_EXISTED, "name", command.Name, HttpStatusCode.Conflict);

            item = await appContext.Albums
                .Where(c => c.Id == command.Id)
                .FirstOrDefaultAsync(cancellationToken);
            if (item == null && command.Id > 0)
                return new FailResult<Result>(ErrorMessages.ALBUM_NOT_FOUND, HttpStatusCode.NotFound);

            ApplicationEvent? evt = null;

            if (item == null)
            {
                item = new Domain.Entities.Album
                {
                };
                appContext.Albums.Add(item);
                evt = new OnAlbumCreatedEvent
                {
                    Ids = [item.Id]
                };
            }
            else
            {
                evt = new OnAlbumUpdatedEvent
                {
                    Ids = [item.Id]
                };
            }

            item.Name = command.Name.Trim();
            item.Description = command.Description;

            await appContext.SaveChangesAsync(cancellationToken);
            await mediator.Publish(evt.SetCurrentUser(currentUser), cancellationToken);

            return new SuccessResult<Result>(new Result
            {
                Id = item.Id,
                CreatedBy = item.CreatedBy,
                CreatedTime = item.CreatedTime,
                Name = item.Name
            });
        }
    }
}