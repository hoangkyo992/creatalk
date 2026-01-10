namespace Cms.Infrastructure.ApiClients.Zns;

internal class ZnsAuthorizationHandler([FromKeyedServices(ZnsAuthorizationHandler.TokenResolverServiceSelector)] IApiTokenResolver tokenResolver)
    : DelegatingHandler
{
    public const string TokenResolverServiceSelector = nameof(ZnsApiTokenResolver);

    protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        var accessToken = await tokenResolver.GetAccessToken();
        request.Headers.TryAddWithoutValidation(tokenResolver.AccessTokenHeaderName, accessToken);

        var context = request.GetPolicyExecutionContext();
        if (context is null)
        {
            context = new Context(Guid.NewGuid().ToString())
            {
                [nameof(IApiTokenResolver)] = tokenResolver
            };
            request.SetPolicyExecutionContext(context);
        }
        else
        {
            context.TryAdd(nameof(IApiTokenResolver), tokenResolver);
        }

        return await base.SendAsync(request, cancellationToken);
    }
}