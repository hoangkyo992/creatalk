namespace Auth.Application.Features.Users;

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
        IAppContext context,
        IMapper mapper,
        IActivityLogService activityLogService,
        ICurrentUser currentUser) : IRequestHandler<Command, ApiResult<Result>>
    {
        public async Task<ApiResult<Result>> Handle(Command command, CancellationToken cancellationToken)
        {
            var item = await context.Users
                .Where(c => c.Id == command.Id)
                .FirstOrDefaultAsync(cancellationToken);
            if (item == null)
                return new FailResult<Result>(ErrorMessages.USER_NOT_FOUND, HttpStatusCode.NotFound);

            activityLogService.Setup(LogLabel.DeleteUser, $"User [{item.Username}] is deleted", currentUser);
            var oldValue = mapper.Map<UserLoggingDto>(item);

            item.IsDeleted = true;
            item.UpdatedBy = currentUser.Username;
            item.UpdatedTime = DateTime.UtcNow;

            activityLogService.AddLog(new LogEntityModel(nameof(User), item.Id)
            {
                Action = ActivityLogAction.Delete,
                OldValue = oldValue,
                NewValue = mapper.Map<UserLoggingDto>(item)
            });

            await context.SaveChangesAsync(cancellationToken);
            await mediator.Publish(new UserEvents.OnDataDeletedEvent
            {
                Ids = [command.Id]
            }.SetCurrentUser(currentUser), cancellationToken);

            await activityLogService.CommitAsync();

            return new SuccessResult<Result>(new Result());
        }
    }
}