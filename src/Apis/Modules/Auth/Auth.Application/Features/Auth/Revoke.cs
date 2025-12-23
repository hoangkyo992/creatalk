namespace Auth.Application.Features.Auth;

public class Revoke
{
    public record Command : IRequest<ApiResult<Result>>
    {
        [JsonConverter(typeof(ZCodeJsonConverter))]
        public long UserId { get; init; }
    }

    public record Result
    {
    }

    public class Validator : AbstractValidator<Command>
    {
        public Validator()
        {
            RuleFor(v => v.UserId)
                .NotNull()
                .NotEmpty();
        }
    }

    public class Handler(IMediator mediator,
        IAppContext context,
        ICurrentUser currentUser) : IRequestHandler<Command, ApiResult<Result>>
    {
        public async Task<ApiResult<Result>> Handle(Command command, CancellationToken cancellationToken)
        {
            var activeSessions = await context.UserSessions
                .Where(c => c.UserId == command.UserId && c.EndTime == null)
                .ToArrayAsync(cancellationToken);
            if (activeSessions.Length != 0)
            {
                await mediator.Publish(new AuthEvents.OnSessionExpiredEvent
                {
                    Actor = currentUser.Username,
                    SessionIds = activeSessions.Select(c => c.Id)
                }.SetCurrentUser(currentUser), cancellationToken);
            }
            return new SuccessResult<Result>(new Result { });
        }
    }
}