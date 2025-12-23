using System.Security.Claims;
using Auth.Application.Common;
using HungHd.Caching;
using HungHd.Shared.Constants;
using Microsoft.AspNetCore.Http;
using Microsoft.IdentityModel.Tokens;
using StackExchange.Redis;

namespace Auth.Application.Features.Auth;

public class SignIn
{
    public class Command : IRequest<ApiResult<Result>>
    {
        public string Username { get; init; }
        public string Password { get; init; }
        public string Platform { get; init; }
    }

    public record Result
    {
        [JsonPropertyName("access_token")]
        public string AccessToken { get; init; }

        [JsonPropertyName("expires_in")]
        public int ExpiresIn { get; init; }

        [JsonPropertyName("token_type")]
        public string TokenType { get; init; } = "Bearer";

        [JsonPropertyName("refresh_token")]
        public string RefreshToken { get; init; }
    }

    public class Validator : AbstractValidator<Command>
    {
        public Validator()
        {
            RuleFor(v => v.Username)
                .NotNull()
                .NotEmpty();
            RuleFor(v => v.Password)
                .NotNull()
                .NotEmpty();
        }
    }

    public class Handler(IMediator mediator,
        IAppContext context,
        ICacheManager cacheManager,
        ITokenService tokenService,
        IHttpContextAccessor httpContextAccessor,
        IOptions<AuthIdentityConfiguration> authIdentityConfigurationOptions) : IRequestHandler<Command, ApiResult<Result>>
    {
        private readonly AuthIdentityConfiguration _authIdentityConfiguration = authIdentityConfigurationOptions.Value
                ?? throw new ArgumentNullException(nameof(authIdentityConfigurationOptions));

        public async Task<ApiResult<Result>> Handle(Command command, CancellationToken cancellationToken)
        {
            var username = command.Username.Trim().ToLower();
            var auth = await context.Users
                .Include(c => c.Roles)
                .IgnoreQueryFilters()
                .Where(c => !c.IsDeleted)
                .Where(c => c.Username == username)
                .AsNoTracking()
                .FirstOrDefaultAsync(cancellationToken);

            if (auth == null)
                return new FailResult<Result>(ErrorMessages.AUTH_NOT_FOUND, "username", command.Username, HttpStatusCode.NotFound);
            if (auth.StatusId == UserStatus.Disabled)
                return new FailResult<Result>(ErrorMessages.AUTH_USER_DISABLED);

            var authDatabase = await cacheManager.GetDatabaseAsync(CacheServer.AuthCache.ToString());

            var attemptKey = CacheKeys.Auth.GetSignInAttemptKey(auth.Id);
            var attemptValue = await authDatabase.StringGetAsync(attemptKey);
            if (attemptValue.HasValue && int.Parse(attemptValue.ToString()) >= _authIdentityConfiguration.MaxFailureSigninAttempt)
                return new FailResult<Result>(ErrorMessages.AUTH_USER_LOCKED_DUETO_MAX_ATTEMPT);

            var msg = ErrorMessages.AUTH_INVALID_PASSWORD;
            if (!PasswordBuilder.Validate(command.Password, auth.Password))
            {
                var attempt = await authDatabase.StringIncrementAsync(attemptKey);
                await authDatabase.KeyExpireAsync(attemptKey, TimeSpan.FromMinutes(_authIdentityConfiguration.FailureLockedIn));
                if (attempt >= _authIdentityConfiguration.MaxFailureSigninAttempt)
                {
                    msg = ErrorMessages.AUTH_INVALID_PASSWORD_AND_LOCKED;
                }
                return new FailResult<Result>(msg, "failureLockedIn", _authIdentityConfiguration.FailureLockedIn, HttpStatusCode.NotAcceptable);
            }
            else
            {
                await authDatabase.KeyDeleteAsync(attemptKey);
            }

            var newSession = new UserSession
            {
                Id = IDGenerator.GenerateId(),
                StartTime = DateTime.UtcNow,
                UserId = auth.Id,
                Username = auth.Username,
                RefreshToken = tokenService.GenerateRefreshToken(),
                IpAddress = httpContextAccessor.HttpContext?.Connection?.RemoteIpAddress?.ToString() ?? string.Empty,
                Navigator = httpContextAccessor.HttpContext?.Request.Headers.UserAgent ?? string.Empty,
                Platform = command.Platform,
                CreatedBy = auth.Username,
                CreatedTime = DateTime.UtcNow
            };
            context.UserSessions.Add(newSession);

            var authIdentity = new ApplicationIdentity
            {
                Id = auth.Id,
                SessionId = newSession.Id,
                IsRobot = false,
                Username = auth.Username,
                Email = auth.Email,
                Avatar = auth.Avatar,
                DisplayName = auth.DisplayName,
                Phone = auth.Phone,
                IsSystemUser = auth.Roles.Select(r => r.RoleId).Contains((long)SystemRole.SuperAdmin),
                PasswordChanged = auth.PasswordChanged
            };

            var authKey = CacheKeys.Auth.GetAuthKey(newSession.Id);
            var expiration = _authIdentityConfiguration.Expiration > 0 ? _authIdentityConfiguration.Expiration : 1440;
            await Task.WhenAll(
                context.SaveChangesAsync(cancellationToken),
                authDatabase.HashSetAsync(authKey,
                [
                    new HashEntry(CacheKeys.Auth.Data, authIdentity.ToJson()),
                    new HashEntry(CacheKeys.Auth.IsssuedTime, DateTime.UtcNow.ToString())
                ]),
                authDatabase.KeyDeleteAsync(attemptKey),
                authDatabase.KeyExpireAsync(authKey, TimeSpan.FromMinutes(expiration))
            );

            var aliveSessions = await context.UserSessions
                .Where(c => c.Id != newSession.Id)
                .Where(c => c.UserId == auth.Id && c.EndTime == null)
                .ToArrayAsync(cancellationToken);
            if (aliveSessions.Length > _authIdentityConfiguration.MaxActiveSession)
            {
                var evt = new AuthEvents.OnSessionExpiredEvent
                {
                    Actor = auth.Username,
                    SessionIds = aliveSessions
                        .OrderByDescending(c => c.StartTime)
                        .Skip(_authIdentityConfiguration.MaxActiveSession - 1)
                        .Select(c => c.Id)
                };
                await mediator.Publish(evt, cancellationToken);
            }

            var claims = new[]
            {
                new Claim(AppClaimTypes.Email, authIdentity.Email),
                new Claim(AppClaimTypes.UserId, ZCode.Get(authIdentity.Id)),
                new Claim(AppClaimTypes.Username, authIdentity.Username),
                new Claim(AppClaimTypes.IssuedAt, ((int)DateTime.UtcNow.Subtract(EpochTime.UnixEpoch).TotalSeconds).ToString(), valueType: ClaimValueTypes.Integer32),
                new Claim(AppClaimTypes.SessionId, ZCode.Get(authIdentity.SessionId))
            };
            return new SuccessResult<Result>(new Result
            {
                AccessToken = tokenService.GenerateAccessToken(claims),
                RefreshToken = newSession.RefreshToken,
                ExpiresIn = expiration * 60
            });
        }
    }
}