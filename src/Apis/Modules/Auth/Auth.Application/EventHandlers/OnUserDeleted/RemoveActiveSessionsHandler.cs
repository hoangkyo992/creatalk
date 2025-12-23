using Microsoft.Extensions.DependencyInjection;

namespace Auth.Application.EventHandlers.OnUserDeleted;

internal class RemoveActiveSessionsHandler(IServiceProvider serviceProvider) : INotificationHandler<UserEvents.OnDataDeletedEvent>
{
    public async Task Handle(UserEvents.OnDataDeletedEvent notification, CancellationToken cancellationToken)
    {
        if (notification.Ids?.Any() != true)
            return;

        using var scope = serviceProvider.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<IAppContext>();
        var sessionIds = await context.UserSessions
            .Where(c => notification.Ids.Contains(c.UserId) && c.EndTime == null)
            .Select(c => c.Id)
            .ToArrayAsync(cancellationToken);
        if (sessionIds.Length != 0)
        {
            var mediator = scope.ServiceProvider.GetRequiredService<IMediator>();
            await mediator.Publish(new AuthEvents.OnSessionExpiredEvent
            {
                Actor = notification.CurrentUser,
                SessionIds = sessionIds
            }.SetCurrentUser(notification.CurrentUser, notification.CurrentUserId), cancellationToken);
        }
    }
}