using System.Security.Claims;
using Microsoft.AspNetCore.Http;

namespace Common.Api.Middleware;

public class TimeZoneMiddleware(RequestDelegate next)
{
    private readonly RequestDelegate _next = next;

    public async Task Invoke(HttpContext context)
    {
        var claims = new List<Claim>();
        var timezoneOffset = context.Request.Headers
            .Where(c => c.Key.Equals(DefaultRequestHeaders.DateTimeOffsetName, StringComparison.OrdinalIgnoreCase))
            .Select(c => c.Value)
            .FirstOrDefault();
        _ = int.TryParse(timezoneOffset, out int offset);
        claims.Add(new Claim(AppClaimTypes.RequestTimeZone, offset.ToString()));
        context.User.AddIdentity(new ClaimsIdentity(claims));
        await _next.Invoke(context);
    }
}