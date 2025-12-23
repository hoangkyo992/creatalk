using Auth.Application.Common;

namespace Auth.Application.Features.Users;

public class ResetPassword
{
    public record Command : IRequest<ApiResult<Result>>
    {
        [JsonIgnore]
        public long UserId { get; set; }
        public string NewPassword { get; init; }
    }

    public record Result
    {
        public bool ReloginNeeded { get; init; }
    }

    public class Validator : AbstractValidator<Command>
    {
        public Validator()
        {
            RuleFor(v => v.UserId)
                .NotNull()
                .NotEmpty()
                .GreaterThan(0);
            RuleFor(v => v.NewPassword)
                .NotNull()
                .NotEmpty();
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
            var user = await context.Users
                .Where(c => c.Id == command.UserId)
                .Where(c => c.Id != currentUser.Id)
                .FirstOrDefaultAsync(cancellationToken);
            if (user == null)
                return new FailResult<Result>(ErrorMessages.USER_NOT_FOUND, HttpStatusCode.NotFound);

            activityLogService.Setup(LogLabel.ResetUserPassword, $"User [{user.Username}]'s password is reset", currentUser);

            var oldValue = mapper.Map<UserLoggingDto>(user);

            user.UpdatedTime = DateTime.UtcNow;
            user.UpdatedBy = currentUser.Username;
            user.Password = PasswordBuilder.Create(command.NewPassword);
            user.PasswordChanged = false;

            activityLogService.AddLog(new LogEntityModel(nameof(User), user.Id)
            {
                Action = ActivityLogAction.Update,
                OldValue = oldValue,
                NewValue = mapper.Map<UserLoggingDto>(user)
            });

            await context.SaveChangesAsync(cancellationToken);

            await mediator.Publish(new UserEvents.OnPasswordChangedEvent
            {
                Ids = [user.Id]
            }.SetCurrentUser(currentUser), cancellationToken);

            await activityLogService.CommitAsync();

            return new SuccessResult<Result>(new Result
            {
                ReloginNeeded = false
            });
        }
    }
}