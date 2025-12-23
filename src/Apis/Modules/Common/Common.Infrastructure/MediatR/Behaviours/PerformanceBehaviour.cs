using System.Diagnostics;

namespace Common.Infrastructure.MediatR.Behaviours;

public class PerformanceBehavior<TRequest, TResponse>(ILogger<TRequest> logger,
    ICurrentUser currentUser) : IPipelineBehavior<TRequest, TResponse> where TRequest : notnull
{
    private readonly Stopwatch _timer = new();

    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
    {
        _timer.Start();
        var response = await next();
        _timer.Stop();
        var elapsedMilliseconds = _timer.ElapsedMilliseconds;
        if (elapsedMilliseconds > 1000)
        {
            var requestName = typeof(TRequest).Name;
            logger.LogWarning("Long Running Request: {Name} ({ElapsedMilliseconds} milliseconds) {@UserId} {@Username} {@Request}",
                requestName, elapsedMilliseconds, currentUser.Id, currentUser.Username, request);
        }
        return response;
    }
}