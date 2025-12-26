namespace Cms.Application.Services.Abstractions;

public interface IQueueService
{
    Task EnqueueAsync(long id);

    ValueTask<long> DequeueAsync(CancellationToken cancellationToken = default);
}