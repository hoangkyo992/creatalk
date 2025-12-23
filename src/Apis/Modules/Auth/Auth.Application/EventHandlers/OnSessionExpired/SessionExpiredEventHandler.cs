using HungHd.Caching;
using Microsoft.Extensions.DependencyInjection;

namespace Auth.Application.EventHandlers.OnSessionExpired;

internal class SessionExpiredEventHandler(IServiceProvider serviceProvider,
    ICacheManager cacheManager) : INotificationHandler<AuthEvents.OnSessionExpiredEvent>
{
    public async Task Handle(AuthEvents.OnSessionExpiredEvent notification, CancellationToken cancellationToken)
    {
        if (notification.SessionIds?.Any() != true)
            return;

        using var scope = serviceProvider.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<IAppContext>();
        var sessions = await context.UserSessions
                      .Where(c => notification.SessionIds.Contains(c.Id) && c.EndTime == null)
                      .ToListAsync(cancellationToken);
        if (sessions.Count != 0)
        {
            foreach (var session in sessions)
            {
                session.EndTime = DateTime.UtcNow;
                session.EndBy = notification.Actor;
            }
            var authDatabase = await cacheManager.GetDatabaseAsync(CacheServer.AuthCache.ToString());
            await context.SaveChangesAsync(cancellationToken);
            await Task.WhenAll(
                 sessions.Select(session => authDatabase.KeyDeleteAsync(CacheKeys.Auth.GetAuthKey(session.Id)))
            );
        }
    }
}