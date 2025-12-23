namespace Auth.Application.Shared.Configurations;

public class AuthIdentityConfiguration
{
    public const string ConfigSection = "AuthIdentity";

    public string Issuer { get; init; }
    public string Audience { get; init; }
    public int Expiration { get; init; }
    public int SessionExpiration { get; init; }
    public string SigningCredential { get; init; }
    public int MaxActiveSession { get; init; }
    public int MaxFailureSigninAttempt { get; init; }
    public int FailureLockedIn { get; init; }
}