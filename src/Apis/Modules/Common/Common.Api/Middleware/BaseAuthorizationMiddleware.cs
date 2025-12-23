using Common.Application.Shared.Attributes;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Features;

namespace Common.Api.Middleware;

public class BaseAuthorizationMiddleware(RequestDelegate next,
    IOptions<ApplicationOptions> applicationOptions)
{
    private readonly RequestDelegate _next = next;
    private readonly ApplicationOptions _applicationOptions = applicationOptions.Value;

    public async Task Invoke(HttpContext context,
        IIdentityService identityService)
    {
        if (!_applicationOptions.DisableAuthorization)
        {
            var endpoint = context.Features.Get<IEndpointFeature>()?.Endpoint;
            if (endpoint != null)
            {
                var allowAnonymousAttribute = endpoint.Metadata.GetMetadata<AllowAnonymousAttribute>();
                if (allowAnonymousAttribute == null)
                {
                    var allowAccessAtrtribute = endpoint.Metadata.GetMetadata<AllowAllUsersAccessAttribute>();
                    if (allowAccessAtrtribute == null)
                    {
                        var checkRightsAttributes = endpoint.Metadata.GetOrderedMetadata<CheckRightsAttribute>();
                        if (checkRightsAttributes.Any())
                        {
                            var features = checkRightsAttributes.SelectMany(c => c.Actions.Select(x => (c.Feature, x.ToUpper()))).ToList();
                            var accessable = await identityService.IsAccessable(features);
                            if (!accessable)
                                throw ExceptionFactory.Create<ForbiddenException>(CommonErrorMessages.UNAUTHORIZED_EXCEPTION, "endpoint", context.Request.Path);
                        }
                    }
                }
            }
        }
        await _next.Invoke(context);
    }
}