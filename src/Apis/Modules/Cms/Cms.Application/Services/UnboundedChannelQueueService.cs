using System.Threading.Channels;
using Cms.Application.Services.Abstractions;

namespace Cms.Application.Services;

public class UnboundedChannelQueueService : IQueueService
{
    private readonly Channel<long> _queue = Channel.CreateUnbounded<long>();

    public async Task EnqueueAsync(long id)
    {
        await _queue.Writer.WriteAsync(id);
    }

    public async ValueTask<long> DequeueAsync(CancellationToken cancellationToken = default)
    {
        return await _queue.Reader.ReadAsync(cancellationToken);
    }
}