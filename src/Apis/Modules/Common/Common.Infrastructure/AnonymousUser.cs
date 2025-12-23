using System.Security.Claims;

namespace Common.Infrastructure;

public class AnonymousUser : ICurrentUser
{
    public bool IsAuthenticated => true;

    public long Id => 1;

    public long SessionId => throw new NotImplementedException();

    public string Username => "N/A";

    public string Phone => "N/A";

    public string Email => "N/A";

    public string AccessToken => throw new NotImplementedException();

    public bool IsRobot => false;

    public TimeSpan ClientDateTimeOffset => throw new NotImplementedException();

    public bool IsSystemUser => throw new NotImplementedException();

    public Claim? FindClaim(string claim)
    {
        throw new NotImplementedException();
    }

    public IEnumerable<Claim> FindClaims(string claim)
    {
        throw new NotImplementedException();
    }

    public IEnumerable<Claim> GetAllClaims()
    {
        throw new NotImplementedException();
    }
}