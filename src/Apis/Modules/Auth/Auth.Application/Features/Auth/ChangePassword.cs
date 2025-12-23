using Auth.Application.Common;
using HungHd.Caching;

namespace Auth.Application.Features.Auth;

public class ChangePassword
{
    public record Command : IRequest<ApiResult<Result>>
    {
        public string Password { get; init; }

        public string NewPassword { get; init; }

        public string ConfirmPassword { get; init; }
    }

    public record Result
    {
        public bool ReloginNeeded { get; init; }
    }

    public class Validator : AbstractValidator<Command>
    {
        public Validator()
        {
            RuleFor(v => v.Password)
                .NotNull()
                .NotEmpty();
            RuleFor(v => v.NewPassword)
                .NotNull()
                .NotEmpty();
            RuleFor(v => v.ConfirmPassword)
                .NotNull()
                .NotEmpty();
            RuleFor(v => v.ConfirmPassword)
                .Equal(v => v.NewPassword);
        }
    }

    public class Handler(IMediator mediator,
        IAppContext context,
        ICurrentUser currentUser,
        IOptions<AuthIdentityConfiguration> authIdentityConfigurationOptions,
        ICacheManager cacheManager) : IRequestHandler<Command, ApiResult<Result>>
    {
        private readonly AuthIdentityConfiguration _authIdentityConfiguration = authIdentityConfigurationOptions.Value
                ?? throw new ArgumentNullException(nameof(authIdentityConfigurationOptions));

        public async Task<ApiResult<Result>> Handle(Command command, CancellationToken cancellationToken)
        {
            var user = await context.Users
                .Where(c => c.Id == currentUser.Id)
                .FirstOrDefaultAsync(cancellationToken);
            if (user == null)
                return new FailResult<Result>(ErrorMessages.USER_NOT_FOUND, HttpStatusCode.NotFound);

            var unauthMessage = ErrorMessages.AUTH_INVALID_PASSWORD;
            var attemptKey = CacheKeys.Auth.GetSignInAttemptKey(user.Id);
            var authDatabase = await cacheManager.GetDatabaseAsync(CacheServer.AuthCache.ToString());

            if (!PasswordBuilder.Validate(command.Password, user.Password))
            {
                var attempt = await authDatabase.StringIncrementAsync(attemptKey);
                if (attempt >= _authIdentityConfiguration.MaxFailureSigninAttempt)
                {
                    unauthMessage = ErrorMessages.AUTH_INVALID_PASSWORD_AND_LOCKED;
                    await authDatabase.KeyExpireAsync(attemptKey, TimeSpan.FromMinutes(_authIdentityConfiguration.FailureLockedIn));
                }
                return new FailResult<Result>(unauthMessage, "failureLockedIn", _authIdentityConfiguration.FailureLockedIn, HttpStatusCode.NotAcceptable);
            }
            else
            {
                await authDatabase.KeyDeleteAsync(attemptKey);
            }

            user.PasswordChanged = true;
            user.Password = PasswordBuilder.Create(command.NewPassword);
            await context.SaveChangesAsync(cancellationToken);

            await mediator.Publish(new UserEvents.OnPasswordChangedEvent
            {
                Ids = [user.Id]
            }.SetCurrentUser(currentUser.Username, currentUser.Id), cancellationToken);

            return new SuccessResult<Result>(new Result
            {
                ReloginNeeded = false
            });
        }
    }
}