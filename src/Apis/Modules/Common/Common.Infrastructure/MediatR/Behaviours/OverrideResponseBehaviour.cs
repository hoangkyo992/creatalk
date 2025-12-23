using System.Diagnostics;

namespace Common.Infrastructure.MediatR.Behaviours;

[DebuggerStepThrough]
public class OverrideResponseBehavior<TRequest, TResponse>(IOptions<ApplicationOptions> options) : IPipelineBehavior<TRequest, TResponse> where TRequest : notnull
{
    private readonly ApplicationOptions _applicationOptions = options.Value;

    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
    {
        var response = await next();
        if (response is not null && _applicationOptions.OverrideResponseStatusCode)
        {
            var method = response
                .GetType()
                .GetMethod("ToInvalidLogicException");
            if (method != null)
            {
                var exception = method.Invoke(response, null);
                if (exception is InvalidLogicException ex)
                    throw ex;
            }
        }
        return response;
    }
}