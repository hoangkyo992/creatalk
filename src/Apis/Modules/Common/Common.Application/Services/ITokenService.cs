using System.Security.Claims;

namespace Common.Application.Services;

public interface ITokenService
{
    long GetSessionId(string token, bool validateLifetime = true);

    string GenerateAccessToken(IEnumerable<Claim> claims);

    string GenerateRefreshToken();
}