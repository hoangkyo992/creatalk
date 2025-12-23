using HungHd.Caching;
using HungHd.Shared.Constants;
using StackExchange.Redis;

namespace Auth.Application.Features.Auth;

public class GetIdentity
{
    public record Request : IRequest<ApiResult<Result>>
    {
        public string Token { get; init; }

        public Request(string token)
        {
            Token = token;
        }
    }

    public record Result
    {
        public ApplicationIdentity AuthData { get; init; }
    }

    public class Validator : AbstractValidator<Request>
    {
        public Validator()
        {
            RuleFor(v => v.Token)
                .NotNull()
                .NotEmpty();
        }
    }

    public class Handler(ICacheManager cacheManager,
        IAppContext context,
        IOptions<AuthIdentityConfiguration> authIdentityConfigurationOptions,
        ITokenService tokenService) : IRequestHandler<Request, ApiResult<Result>>
    {
        private readonly AuthIdentityConfiguration _authIdentityConfiguration = authIdentityConfigurationOptions.Value
                ?? throw new ArgumentNullException(nameof(authIdentityConfigurationOptions));

        public async Task<ApiResult<Result>> Handle(Request request, CancellationToken cancellationToken)
        {
            var sessionId = tokenService.GetSessionId(request.Token);
            var authDatabase = await cacheManager.GetDatabaseAsync(CacheServer.AuthCache.ToString());
            var authData = (await authDatabase.HashGetAllAsync(CacheKeys.Auth.GetAuthKey(sessionId)))?.ToList();
            if (authData == null || authData.Count == 0 || !authData.Exists(c => c.Name == CacheKeys.Auth.Data))
            {
                // Find & set session to cache
                var session = await context.UserSessions
                    .FirstOrDefaultAsync(c => c.Id == sessionId && c.EndTime == null, cancellationToken);
                if (session != null)
                {
                    var auth = await context.Users
                        .Include(c => c.Roles)
                        .IgnoreQueryFilters()
                        .Where(c => !c.IsDeleted)
                        .AsNoTracking()
                        .FirstOrDefaultAsync(c => c.Id == session.UserId && c.StatusId == UserStatus.Active, cancellationToken);
                    if (auth != null)
                    {
                        var authIdentity = new ApplicationIdentity
                        {
                            Id = session.UserId,
                            SessionId = session.Id,
                            IsRobot = false,
                            Username = auth.Username,
                            Email = auth.Email,
                            Avatar = auth.Avatar,
                            DisplayName = auth.DisplayName,
                            Phone = auth.Phone,
                            IsSystemUser = auth.Roles.Select(r => r.RoleId).Contains((long)SystemRole.SuperAdmin),
                            PasswordChanged = auth.PasswordChanged
                        };
                        var authKey = CacheKeys.Auth.GetAuthKey(authIdentity.SessionId);
                        var expiration = _authIdentityConfiguration.Expiration > 0 ? _authIdentityConfiguration.Expiration : 1440;
                        await Task.WhenAll(
                            context.SaveChangesAsync(cancellationToken),
                            authDatabase.HashSetAsync(authKey,
                            [
                                new HashEntry(CacheKeys.Auth.Data, authIdentity.ToJson()),
                                new HashEntry(CacheKeys.Auth.IsssuedTime, DateTime.UtcNow.ToString())
                            ]),
                            authDatabase.KeyExpireAsync(authKey, TimeSpan.FromMinutes(expiration))
                        );

                        return new SuccessResult<Result>(new Result
                        {
                            AuthData = authIdentity
                        });
                    }
                }
                return new FailResult<Result>(CommonErrorMessages.UNAUTHENTICATED_EXCEPTION, HttpStatusCode.Unauthorized);
            }

            return new SuccessResult<Result>(new Result
            {
                AuthData = authData.Find(c => c.Name == CacheKeys.Auth.Data).Value.ToString().FromJson<ApplicationIdentity>()
            });
        }
    }
}