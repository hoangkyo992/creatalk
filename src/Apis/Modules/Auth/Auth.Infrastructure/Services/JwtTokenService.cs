using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using Auth.Application.Configurations;
using Common.Application.Services;
using HungHd.Shared;
using HungHd.Shared.Constants;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace Auth.Infrastructure.Services;

public class JwtTokenService(IOptions<AuthIdentityConfiguration> authIdentityConfiguration,
    ILogger<JwtTokenService> logger) : ITokenService
{
    private readonly AuthIdentityConfiguration _authIdentityConfiguration = authIdentityConfiguration.Value
            ?? throw new ArgumentNullException(nameof(authIdentityConfiguration));

    private readonly ILogger<JwtTokenService> _logger = logger ?? throw new ArgumentNullException(nameof(logger));

    public long GetSessionId(string token, bool validateLifetime = true)
    {
        var principal = GetClaimsPrincipal(token, validateLifetime, out var securityToken);
        if (principal == null || principal.Identity == null || principal.Claims?.Any() != true)
            return -1;
        var claim = principal.Claims.FirstOrDefault(c => c.Type == AppClaimTypes.SessionId);
        if (claim == null || !ZCode.TryGetInt64(claim.Value.ToString(), out long sessionId) || sessionId <= 0)
            return -1;
        return sessionId;
    }

    public string GenerateRefreshToken()
    {
        var randomNumber = new byte[32];
        using var rng = RandomNumberGenerator.Create();
        rng.GetBytes(randomNumber);
        return Convert.ToBase64String(randomNumber);
    }

    public string GenerateAccessToken(IEnumerable<Claim> claims)
    {
        SigningCredentials? credentials = null;
        if (!string.IsNullOrWhiteSpace(_authIdentityConfiguration.SigningCredential))
        {
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_authIdentityConfiguration.SigningCredential));
            credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);
        }
        var securityToken = new JwtSecurityToken(_authIdentityConfiguration.Issuer,
            _authIdentityConfiguration.Audience,
            claims,
            expires: DateTime.UtcNow.AddMinutes(_authIdentityConfiguration.Expiration),
            signingCredentials: credentials);
        var token = new JwtSecurityTokenHandler().WriteToken(securityToken);
        return token;
    }

    private ClaimsPrincipal? GetClaimsPrincipal(string token, bool validateLifetime, out SecurityToken? securityToken)
    {
        securityToken = null;
        try
        {
            return new JwtSecurityTokenHandler()
                .ValidateToken(token,
                    GetValidationParameters(validateLifetime),
                    out securityToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Validate token failed. Error: {Message}", ex.Message);
        }
        return null;
    }

    private TokenValidationParameters GetValidationParameters(bool validateLifetime)
    {
        var validationParameters = new TokenValidationParameters()
        {
            ValidateLifetime = validateLifetime,
            ValidateAudience = false,
            ValidateIssuer = false,
            ValidIssuer = _authIdentityConfiguration.Issuer,
            ValidAudience = _authIdentityConfiguration.Audience
        };
        if (!string.IsNullOrWhiteSpace(_authIdentityConfiguration.SigningCredential))
        {
            validationParameters.IssuerSigningKey =
                new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_authIdentityConfiguration.SigningCredential));
        }

        return validationParameters;
    }
}