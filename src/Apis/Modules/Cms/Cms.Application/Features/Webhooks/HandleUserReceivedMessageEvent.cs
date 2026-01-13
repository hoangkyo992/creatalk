namespace Cms.Application.Features.Webhooks;

public class HandleUserReceivedMessageEvent
{
    public record Command : IRequest<ApiResult<Result>>
    {
        public long ProviderId { get; init; }
        public string ProviderCode { get; init; }
        public Dictionary<string, object> Parameters { get; init; } = [];
        public string EventPayload { get; init; }
        public string EventName { get; init; }
    }

    public record Result
    {
    }

    public class Handler(IAppContext appContext, IServiceProvider serviceProvider) : IRequestHandler<Command, ApiResult<Result>>
    {
        public async Task<ApiResult<Result>> Handle(Command command, CancellationToken cancellationToken)
        {
            var service = serviceProvider.GetKeyedService<IMessageSender>(command.ProviderCode);
            if (service is null)
                return new FailResult<Result>($"The message sender {command.ProviderCode} is not implemented!", HttpStatusCode.NotImplemented);

            var eventPayload = await service.GetEventDataAsync(command.EventName, command.EventPayload, cancellationToken);
            if (eventPayload.Count == 0)
                return new FailResult<Result>($"Failed to get event data!", HttpStatusCode.NotAcceptable);

            var msgId = eventPayload.GetValueOrDefault("MessageId")?.ToString() ?? string.Empty;

            var message = await appContext.AttendeeMessages
                .IgnoreQueryFilters()
                .Where(c => c.StatusId == MessageStatus.Succeeded)
                .Where(c => c.MessageId == msgId)
                .Where(c => c.ProviderId == command.ProviderId)
                .FirstOrDefaultAsync(cancellationToken);

            if (message == null)
                return new FailResult<Result>(ErrorMessages.MESSAGE_NOT_FOUND, HttpStatusCode.NotFound);

            message.MessageId = msgId;
            message.EventPayload = command.EventPayload;
            message.UserReceivedAt = (DateTime)eventPayload.GetValueOrDefault("DeliveryTime")!;
            message.StatusId = MessageStatus.UserReceived;

            await appContext.SaveChangesAsync(cancellationToken);

            return new SuccessResult<Result>(new Result());
        }
    }
}