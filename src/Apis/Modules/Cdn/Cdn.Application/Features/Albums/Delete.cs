using AutoMapper;
using Common.Domain.Enums;

namespace Cdn.Application.Features.Albums;

public class Delete
{
    public record Command : IRequest<ApiResult<Result>>
    {
        [JsonIgnore]
        public long Id { get; set; }
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
        IMapper mapper,
        IActivityLogService activityLogService,
        ICurrentUser currentUser) : IRequestHandler<Command, ApiResult<Result>>
    {
        public async Task<ApiResult<Result>> Handle(Command command, CancellationToken cancellationToken)
        {
            var item = await appContext.Albums
                .Where(c => c.Id == command.Id)
                .FirstOrDefaultAsync(cancellationToken);
            if (item == null)
                return new FailResult<Result>(ErrorMessages.ALBUM_NOT_FOUND, HttpStatusCode.NotFound);

            activityLogService.Setup(LogLabel.DeleteAlbum, $"Album [{item.Name}] is deleted", currentUser);
            var oldValue = mapper.Map<AlbumLoggingDto>(item);

            item.IsDeleted = true;

            activityLogService.AddLog(new LogEntityModel(nameof(Album), item.Id)
            {
                Action = ActivityLogAction.Delete,
                OldValue = oldValue,
                NewValue = new { }
            });

            await appContext.SaveChangesAsync(cancellationToken);
            await mediator.Publish(new OnAlbumDeletedEvent
            {
                Ids = [item.Id]
            }.SetCurrentUser(currentUser), cancellationToken);

            await activityLogService.CommitAsync();

            return new SuccessResult<Result>(new Result());
        }
    }
}