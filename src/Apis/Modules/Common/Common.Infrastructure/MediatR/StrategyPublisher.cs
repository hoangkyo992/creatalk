using Common.Application.Shared;

namespace Common.Infrastructure.MediatR;

public class StrategyPublisher : INotificationPublisher
{
    public async Task Publish(IEnumerable<NotificationHandlerExecutor> handlerExecutors, INotification notification, CancellationToken cancellationToken)
    {
        if (notification is ApplicationEvent applicationEvent)
        {
            handlerExecutors = handlerExecutors.OrderBy(c => c is IOrderedObject notificationHandler ? notificationHandler.Order : int.MaxValue);

            if (applicationEvent.PublishStrategy == PublishStrategy.Async)
            {
                await AsyncContinueOnException(handlerExecutors, notification, cancellationToken);
            }
            else if (applicationEvent.PublishStrategy == PublishStrategy.SyncStopOnException)
            {
                await SyncStopOnException(handlerExecutors, notification, cancellationToken);
            }
            else if (applicationEvent.PublishStrategy == PublishStrategy.SyncContinueOnException)
            {
                await SyncContinueOnException(handlerExecutors, notification, cancellationToken);
            }
            else if (applicationEvent.PublishStrategy == PublishStrategy.ParallelNoWait)
            {
                _ = ParallelNoWait(handlerExecutors, notification, cancellationToken);
            }
#pragma warning disable IDE0045 // Convert to conditional expression
            else if (applicationEvent.PublishStrategy == PublishStrategy.ParallelWhenAny)
            {
                _ = ParallelWhenAny(handlerExecutors, notification, cancellationToken);
            }
            else if (applicationEvent.PublishStrategy == PublishStrategy.ParallelWhenAll)
            {
                _ = ParallelWhenAll(handlerExecutors, notification, cancellationToken);
            }
            else
            {
                throw new NotImplementedException("Unknown publish strategy.");
            }
#pragma warning restore IDE0045 // Convert to conditional expression
        }
        else
        {
            throw new NotImplementedException("Notification event class should inherit ApplicationEvent.");
        }
    }

    private static Task ParallelWhenAll(IEnumerable<NotificationHandlerExecutor> handlers, INotification notification, CancellationToken cancellationToken)
    {
        var tasks = new List<Task>();
        foreach (var handler in handlers)
        {
            tasks.Add(Task.Run(() => handler.HandlerCallback(notification, cancellationToken), cancellationToken));
        }

        return Task.WhenAll(tasks);
    }

    private static Task<Task> ParallelWhenAny(IEnumerable<NotificationHandlerExecutor> handlers, INotification notification, CancellationToken cancellationToken)
    {
        var tasks = new List<Task>();
        foreach (var handler in handlers)
        {
            tasks.Add(Task.Run(() => handler.HandlerCallback(notification, cancellationToken), cancellationToken));
        }

        return Task.WhenAny(tasks);
    }

    private static Task ParallelNoWait(IEnumerable<NotificationHandlerExecutor> handlers, INotification notification, CancellationToken cancellationToken)
    {
        foreach (var handler in handlers)
        {
            Task.Run(() => handler.HandlerCallback(notification, cancellationToken), cancellationToken);
        }

        return Task.CompletedTask;
    }

    private static async Task AsyncContinueOnException(IEnumerable<NotificationHandlerExecutor> handlers, INotification notification, CancellationToken cancellationToken)
    {
        var tasks = new List<Task>();
        var exceptions = new List<Exception>();
        foreach (var handler in handlers)
        {
            try
            {
                tasks.Add(handler.HandlerCallback(notification, cancellationToken));
            }
            catch (Exception ex) when (!(ex is OutOfMemoryException || ex is StackOverflowException))
            {
                exceptions.Add(ex);
            }
        }

        try
        {
            await Task.WhenAll(tasks).ConfigureAwait(false);
        }
        catch (AggregateException ex)
        {
            exceptions.AddRange(ex.Flatten().InnerExceptions);
        }
        catch (Exception ex) when (!(ex is OutOfMemoryException || ex is StackOverflowException))
        {
            exceptions.Add(ex);
        }

        if (exceptions.Count != 0)
        {
            throw new AggregateException(exceptions);
        }
    }

    private static async Task SyncStopOnException(IEnumerable<NotificationHandlerExecutor> handlers, INotification notification, CancellationToken cancellationToken)
    {
        foreach (var handler in handlers)
        {
            await handler.HandlerCallback(notification, cancellationToken).ConfigureAwait(false);
        }
    }

    private static async Task SyncContinueOnException(IEnumerable<NotificationHandlerExecutor> handlers, INotification notification, CancellationToken cancellationToken)
    {
        var exceptions = new List<Exception>();

        foreach (var handler in handlers)
        {
            try
            {
                await handler.HandlerCallback(notification, cancellationToken).ConfigureAwait(false);
            }
            catch (AggregateException ex)
            {
                exceptions.AddRange(ex.Flatten().InnerExceptions);
            }
            catch (Exception ex) when (!(ex is OutOfMemoryException || ex is StackOverflowException))
            {
                exceptions.Add(ex);
            }
        }

        if (exceptions.Count != 0)
        {
            throw new AggregateException(exceptions);
        }
    }
}