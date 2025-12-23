namespace Cdn.Application.EventHandlers.OnActivityLogCreated;

internal class CreateActivityLogEventHandler() : INotificationHandler<ActivityLogEventItem>
{
    public async Task Handle(ActivityLogEventItem notification,
        CancellationToken cancellationToken)
    {
        if (notification.LogEntities == null || !notification.LogEntities.Any())
            return;

        await Task.CompletedTask;
    }
}