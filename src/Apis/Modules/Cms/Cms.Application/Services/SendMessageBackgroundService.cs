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
                    .IgnoreQueryFilters()
                    .Where(c => c.StatusId == MessageStatus.New)
                    .Where(c => c.Id == id)
                    .Include(c => c.Attendee)
                    .Include(c => c.Provider)
                    .FirstOrDefaultAsync(stoppingToken);

                if (message is not null)
                {
                    message.StatusId = MessageStatus.Sending;
                    message.SentAt = DateTime.UtcNow;
                    await appContext.SaveChangesAsync(CancellationToken.None);

                    try
                    {
                        var service = scope.ServiceProvider.GetKeyedService<IMessageSender>(message.Provider.Code)
                            ?? throw new NotImplementedException($"The message sender {message.Provider.Code} is not implemented!");

                        var output = await service.SendAsync(message.Provider, message, CancellationToken.None);

                        message.StatusId = output.IsSuccess ? MessageStatus.Succeeded : MessageStatus.Failed;
                        message.MessageId = output.MessageId;
                        message.RequestPayload = output.RequestPayload;
                        message.ResponsePayload = output.ResponsePayload;
                    }
                    catch (Exception ex)
                    {
                        message.StatusId = MessageStatus.Failed;
                        message.ResponsePayload = ex.ToString();
                    }
                    finally
                    {
                        await appContext.SaveChangesAsync(CancellationToken.None);
                        await Task.Delay(100, stoppingToken);
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