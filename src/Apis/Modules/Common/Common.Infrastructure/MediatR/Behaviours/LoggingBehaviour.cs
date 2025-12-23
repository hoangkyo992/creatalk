namespace Common.Infrastructure.MediatR.Behaviours;

public class LoggingBehavior<TRequest>(ILogger<TRequest> logger, ICurrentUser currentUser) : IRequestPreProcessor<TRequest> where TRequest : notnull
{
    public async Task Process(TRequest request, CancellationToken cancellationToken)
    {
        var requestName = typeof(TRequest).Name;
        logger.LogInformation("Request: {Name} {@UserId} {@Username} {@Request}",
            requestName, currentUser.Id, currentUser.Username, request);
        await Task.CompletedTask;
    }
}