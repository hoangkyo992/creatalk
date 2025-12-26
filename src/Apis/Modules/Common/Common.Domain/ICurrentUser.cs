using System.Security.Claims;

namespace Common.Domain;

public interface ICurrentUser
{
    bool IsAuthenticated { get; }
    long Id { get; }
    long SessionId { get; }
    string Username { get; }
    string Phone { get; }
    string Email { get; }
    string AccessToken { get; }
    bool IsRobot { get; }
    bool IsSystemUser { get; }
    long TenantId { get; }

    TimeSpan ClientDateTimeOffset { get; }

    Claim? FindClaim(string claim);

    IEnumerable<Claim> FindClaims(string claim);

    IEnumerable<Claim> GetAllClaims();
}