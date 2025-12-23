using System.Security.Claims;
using HungHd.Caching;
using HungHd.Shared.Constants;
using Microsoft.AspNetCore.Http;
using Microsoft.IdentityModel.Tokens;
using StackExchange.Redis;

namespace Auth.Application.Features.Auth;

public class Renew
{
    public record Command : IRequest<ApiResult<Result>>
    {
        public string AccessToken { get; init; }
        public string RefreshToken { get; init; }
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
            RuleFor(v => v.AccessToken)
                .NotNull()
                .NotEmpty();
            RuleFor(v => v.RefreshToken)
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
            var sessionId = tokenService.GetSessionId(command.AccessToken, validateLifetime: false);
            var currentSession = await context.UserSessions
                .Where(c => c.Id == sessionId && !c.EndTime.HasValue && c.RefreshToken == command.RefreshToken)
                .FirstOrDefaultAsync(cancellationToken);
            if (currentSession == null)
            {
                return new FailResult<Result>(ErrorMessages.AUTH_REFRESH_TOKEN_NOT_FOUND, HttpStatusCode.NotFound);
            }
            var sessionExpiration = _authIdentityConfiguration.SessionExpiration > 0 ? _authIdentityConfiguration.SessionExpiration : 1440;
            if (currentSession.CreatedTime.AddMinutes(sessionExpiration) <= DateTime.UtcNow)
            {
                return new FailResult<Result>(ErrorMessages.AUTH_REFRESH_TOKEN_EXPIRED);
            }

            var auth = await context.Users
                .Include(c => c.Roles)
                .IgnoreQueryFilters()
                .Where(c => !c.IsDeleted)
                .Where(c => c.Id == currentSession.UserId)
                .AsNoTracking()
                .FirstOrDefaultAsync(cancellationToken);

            if (auth == null)
                return new FailResult<Result>(ErrorMessages.AUTH_NOT_FOUND, HttpStatusCode.NotFound);
            if (auth.StatusId == UserStatus.Disabled)
                return new FailResult<Result>(ErrorMessages.AUTH_USER_DISABLED);

            var newSession = new UserSession
            {
                Id = IDGenerator.GenerateId(),
                StartTime = DateTime.UtcNow,
                UserId = currentSession.UserId,
                Username = currentSession.Username,
                RefreshToken = tokenService.GenerateRefreshToken(),
                IpAddress = httpContextAccessor.HttpContext?.Connection?.RemoteIpAddress?.ToString() ?? string.Empty,
                Navigator = httpContextAccessor.HttpContext?.Request.Headers.UserAgent ?? string.Empty,
                Platform = currentSession.Platform,
                CreatedBy = currentSession.Username,
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
            var authDatabase = await cacheManager.GetDatabaseAsync(CacheServer.AuthCache.ToString());
            var expiration = _authIdentityConfiguration.Expiration > 0 ? _authIdentityConfiguration.Expiration : 1440;
            await Task.WhenAll(
                authDatabase.HashSetAsync(authKey,
                [
                    new HashEntry(CacheKeys.Auth.Data, authIdentity.ToJson()),
                    new HashEntry(CacheKeys.Auth.IsssuedTime, DateTime.UtcNow.ToString())
                ]),
                authDatabase.KeyExpireAsync(authKey, TimeSpan.FromMinutes(expiration)),
                context.SaveChangesAsync(cancellationToken)
            );

            var aliveSessions = await context.UserSessions
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
                        .Concat([sessionId])
                        .Distinct()
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