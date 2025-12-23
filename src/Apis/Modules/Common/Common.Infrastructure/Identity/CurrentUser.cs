using System.Security.Claims;
using Microsoft.AspNetCore.Http;

namespace Common.Infrastructure.Identity;

public class CurrentUser : ICurrentUser
{
    private readonly ClaimsPrincipal _claimsPrincipal;

    public CurrentUser(IHttpContextAccessor contextAccessor)
    {
        if (contextAccessor.HttpContext?.User == null)
            throw new UnauthorizedAccessException();
        _claimsPrincipal = contextAccessor.HttpContext.User;
    }

    public bool IsAuthenticated => _claimsPrincipal is not null
        && _claimsPrincipal.Identity?.IsAuthenticated == true
        && _claimsPrincipal.FindUserId() > 0;

    public long Id => _claimsPrincipal.FindUserId();

    public long SessionId => _claimsPrincipal.GetClaimValue<long>(AppClaimTypes.SessionId);

    public long RequestApplicationId => _claimsPrincipal.GetClaimValue<long>(AppClaimTypes.RequestApplicationId);

    public string Username => _claimsPrincipal.FindClaimValue(AppClaimTypes.Username);

    public string Phone => _claimsPrincipal.FindClaimValue(AppClaimTypes.Phone);

    public string Email => _claimsPrincipal.FindClaimValue(AppClaimTypes.Email);

    public bool IsRobot => string.Equals(_claimsPrincipal.FindClaimValue(AppClaimTypes.IsRobot), "True", StringComparison.InvariantCultureIgnoreCase);

    public string AccessToken => _claimsPrincipal.FindClaimValue(AppClaimTypes.AccessToken);

    public TimeSpan ClientDateTimeOffset
    {
        get
        {
            var timezoneClaim = FindClaim(AppClaimTypes.RequestTimeZone);
            if (timezoneClaim != null)
            {
                return int.TryParse(timezoneClaim.Value, out var timeOffset)
                    ? TimeSpan.FromMinutes(-timeOffset)
                    : TimeZoneInfo.FindSystemTimeZoneById(timezoneClaim.Value).BaseUtcOffset;
            }
            else
            {
                // UTC+7
                return TimeSpan.FromMinutes(-420);
            }
        }
    }

    public bool IsSystemUser => string.Equals(_claimsPrincipal.FindClaimValue("ssu"), "True", StringComparison.InvariantCultureIgnoreCase);

    public Claim? FindClaim(string claim) => _claimsPrincipal.Claims.FirstOrDefault(x => x.Type == claim);

    public IEnumerable<Claim> FindClaims(string claim) => _claimsPrincipal.Claims.Where(x => x.Type == claim);

    public IEnumerable<Claim> GetAllClaims() => _claimsPrincipal.Claims ?? [];
}