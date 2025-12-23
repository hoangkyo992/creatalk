namespace Auth.Application.Features.Auth;

public class SignOut
{
    public record Command : IRequest<ApiResult<Result>>
    {
    }

    public record Result
    {
    }

    public class Handler(IMediator mediator,
        ICurrentUser currentUser) : IRequestHandler<Command, ApiResult<Result>>
    {
        public async Task<ApiResult<Result>> Handle(Command command, CancellationToken cancellationToken)
        {
            await mediator.Publish(new AuthEvents.OnSessionExpiredEvent
            {
                Actor = currentUser.Username,
                SessionIds = [currentUser.SessionId]
            }.SetCurrentUser(currentUser.Username, currentUser.Id), cancellationToken);
            return new SuccessResult<Result>(new Result());
        }
    }
}