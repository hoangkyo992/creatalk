namespace Cms.Application.Services;

public class EnqueueMessagesBackgroundService(IServiceScopeFactory scopeFactory,
    IQueueService queueService,
    ILogger<EnqueueMessagesBackgroundService> logger) : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        try
        {
            using var scope = scopeFactory.CreateScope();
            var appContext = scope.ServiceProvider.GetRequiredService<IAppContext>();
            var messages = await appContext.AttendeeMessages
                .Where(c => c.StatusId == MessageStatus.New)
                .OrderBy(c => c.Id)
                .Select(c => c.Id)
                .ToListAsync(stoppingToken);
            if (messages.Count > 0)
            {
                foreach (var id in messages)
                {
                    await queueService.EnqueueAsync(id);
                }
            }
        }
        catch (Exception ex)
        {
            logger.LogError(ex, nameof(EnqueueMessagesBackgroundService));
        }
    }
}