using System.Security.Claims;
using System.Text.Encodings.Web;
using Microsoft.AspNetCore.Authentication;

namespace Common.Api.Middleware.AuthenticationSchemes;

public class NullAuthenticationSchemeOptions : AuthenticationSchemeOptions
{
    public const string SchemeName = "NullAuthenticationScheme";
}

public class NullAuthenticationHandler(IOptionsMonitor<NullAuthenticationSchemeOptions> options,
    ILoggerFactory loggerFactory, UrlEncoder encoder) : AuthenticationHandler<NullAuthenticationSchemeOptions>(options, loggerFactory, encoder)
{
    protected override async Task<AuthenticateResult> HandleAuthenticateAsync()
    {
        var claims = new List<Claim>
        {
        };
        var claimsIdentity = new ClaimsIdentity(claims, nameof(NullAuthenticationHandler));
        var ticket = new AuthenticationTicket(new ClaimsPrincipal(claimsIdentity), new AuthenticationProperties(), Scheme.Name);
        return await Task.FromResult(AuthenticateResult.Success(ticket));
    }
}