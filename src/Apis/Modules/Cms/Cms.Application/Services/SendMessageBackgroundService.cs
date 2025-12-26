using Cms.Application.Services.Abstractions;

namespace Cms.Application.Services;

public class SendMessageBackgroundService(IServiceScopeFactory scopeFactory,
    IQueueService queueService,
    ILogger<SendMessageBackgroundService> logger) : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            var id = await queueService.DequeueAsync(stoppingToken);
            try
            {
                using var scope = scopeFactory.CreateScope();
                var appContext = scope.ServiceProvider.GetRequiredService<IAppContext>();
                var message = await appContext.AttendeeMessages
                    .Where(c => c.StatusId == MessageStatus.New)
                    .Where(c => c.Id == id)
                    .Include(c => c.Provider)
                    .FirstOrDefaultAsync(stoppingToken);

                if (message is not null)
                {
                    message.StatusId = MessageStatus.Sending;
                    await appContext.SaveChangesAsync();

                    try
                    {
                        var service = scope.ServiceProvider.GetKeyedService<IMessageSender>(message.Provider.Code);
                        if (service is null)
                            throw new NotImplementedException($"The message sender {message.Provider.Code} is not implemented!");

                        //await service.SendAsync(id, stoppingToken);

                        message.StatusId = MessageStatus.Succeeded;
                        await appContext.SaveChangesAsync();
                    }
                    catch (Exception ex)
                    {
                        message.StatusId = MessageStatus.Failed;
                        message.ResponsePayload = ex.ToString();
                        await appContext.SaveChangesAsync();
                    }
                }
            }
            catch (Exception ex)
            {
                logger.LogError(ex, nameof(SendMessageBackgroundService));
            }
        }
    }
}