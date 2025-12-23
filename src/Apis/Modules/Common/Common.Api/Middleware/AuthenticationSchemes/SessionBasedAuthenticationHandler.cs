using System.Security.Claims;
using System.Text.Encodings.Web;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Primitives;

namespace Common.Api.Middleware.AuthenticationSchemes;

public class SessionBasedAuthenticationSchemeOptions : AuthenticationSchemeOptions
{
    public const string SchemeName = "SessionBasedAuthenticationScheme";
}

public class SessionBasedAuthenticationHandler(IOptionsMonitor<SessionBasedAuthenticationSchemeOptions> options,
    ILoggerFactory loggerFactory,
    UrlEncoder encoder,
    IIdentityService identityService) : AuthenticationHandler<SessionBasedAuthenticationSchemeOptions>(options, loggerFactory, encoder)
{
    private readonly IIdentityService _identityService = identityService;

    protected override async Task<AuthenticateResult> HandleAuthenticateAsync()
    {
        Request.Headers.TryGetValue(DefaultRequestHeaders.Authorization, out StringValues tokens);
        if (string.IsNullOrWhiteSpace(tokens) || !tokens.ToString().StartsWith("Bearer"))
            return AuthenticateResult.Fail(CommonErrorMessages.UNAUTHENTICATED_EXCEPTION);

        var token = tokens.ToString().Split(" ")[1].Trim();
        if (string.IsNullOrWhiteSpace(token))
            return AuthenticateResult.Fail(CommonErrorMessages.UNAUTHENTICATED_EXCEPTION);

        var auth = await _identityService.GetIdentity(token);
        if (auth == null || auth.Id <= 0)
            return AuthenticateResult.Fail(CommonErrorMessages.UNAUTHENTICATED_EXCEPTION);

        var claims = new List<Claim>
        {
            new(AppClaimTypes.AccessToken, token),
            new(AppClaimTypes.UserId, auth.Id.ToString()),
            new(AppClaimTypes.Username, auth.Username),
            new(AppClaimTypes.Phone, auth.Phone ?? string.Empty),
            new(AppClaimTypes.Email, auth.Email),
            new(AppClaimTypes.SessionId, auth.SessionId.ToString()),
            new(AppClaimTypes.IsRobot, auth.IsRobot.ToString()),
            new("ssu", auth.IsSystemUser.ToString()),
        };

        var claimsIdentity = new ClaimsIdentity(claims, nameof(SessionBasedAuthenticationHandler));
        var ticket = new AuthenticationTicket(new ClaimsPrincipal(claimsIdentity), new AuthenticationProperties(), Scheme.Name);
        return AuthenticateResult.Success(ticket);
    }
}