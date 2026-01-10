namespace Cms.Application.Features.Webhooks;

public class HandleEvent
{
    public record Command : IRequest<ApiResult<Result>>
    {
        public long ProviderId { get; init; }
        public string ProviderCode { get; init; }
        public Dictionary<string, object> Parameters { get; init; } = [];
        public string EventPayload { get; init; }
        public string MessageId { get; init; }
        public string TrackingId { get; init; }
        public DateTime DeliveryTime { get; init; }
    }

    public record Result
    {
    }

    public class Handler(IAppContext appContext) : IRequestHandler<Command, ApiResult<Result>>
    {
        public async Task<ApiResult<Result>> Handle(Command command, CancellationToken cancellationToken)
        {
            var message = await appContext.AttendeeMessages
                .IgnoreQueryFilters()
                .Where(c => c.StatusId == MessageStatus.Succeeded)
                .Where(c => c.MessageId == command.MessageId)
                .Where(c => c.ProviderId == command.ProviderId)
                .FirstOrDefaultAsync(cancellationToken);

            if (message == null)
                return new FailResult<Result>(ErrorMessages.MESSAGE_NOT_FOUND, HttpStatusCode.NotFound);

            message.MessageId = command.MessageId;
            message.EventPayload = command.EventPayload;
            message.UserReceivedAt = command.DeliveryTime;
            message.StatusId = MessageStatus.UserReceived;

            await appContext.SaveChangesAsync(cancellationToken);

            return new SuccessResult<Result>(new Result());
        }
    }
}