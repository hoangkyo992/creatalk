using System.Security.Claims;
using Microsoft.AspNetCore.Http;

namespace Common.Api.Middleware;

public class TenantIdentificationMiddleware(RequestDelegate next)
{
    private readonly RequestDelegate _next = next;

    public async Task Invoke(HttpContext context)
    {
        var tenantValue = context.Request.Headers
                .Where(c => c.Key.Equals(DefaultRequestHeaders.TenantIdName, StringComparison.OrdinalIgnoreCase))
                .Select(c => c.Value.ToString())
                .FirstOrDefault() ?? string.Empty;

        ZCode.TryGetInt64(tenantValue, out var tenantId);
        var claims = new List<Claim>
            {
                new(AppClaimTypes.TenantId, tenantId.ToString())
            };
        context.User.AddIdentity(new ClaimsIdentity(claims));

        await _next.Invoke(context);
    }
}