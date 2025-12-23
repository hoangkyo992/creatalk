using System.Reflection;
using Common.Application.Shared.Attributes;
using Common.Application.Shared.Exceptions;
using Polly;

namespace Common.Infrastructure.MediatR.Behaviours;

public class RetryPolicyBehavior<TRequest, TResponse>(ILogger<RetryPolicyBehavior<TRequest, TResponse>> logger,
    IRequestHandler<TRequest, TResponse> handler) : IPipelineBehavior<TRequest, TResponse> where TRequest : IRequest<TResponse>
{
    public async Task<TResponse> Handle(TRequest request,
        RequestHandlerDelegate<TResponse> next,
        CancellationToken cancellationToken)
    {
        var retryAttr = handler.GetType().GetCustomAttribute<RetryPolicyAttribute>();
        return retryAttr == null
            ? await next()
            : await Policy.Handle<RetryableException>()
            .WaitAndRetryAsync(retryAttr.RetryCount,
                i => TimeSpan.FromMilliseconds(i * i * retryAttr.SleepDuration),
                (ex, ts, _) => logger.LogWarning(ex, "Failed to execute handler for request {Request}, retrying after {RetryTimeSpan}s: {ExceptionMessage}", typeof(TRequest).Name, ts.TotalSeconds, ex.Message))
            .ExecuteAsync(async () => await next());
    }
}