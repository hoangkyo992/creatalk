namespace Cms.Application.Features.Webhooks;

public class VerifyEvent
{
    public record Command : IRequest<ApiResult<Result>>
    {
        public long ProviderId { get; init; }
        public string ProviderCode { get; init; }
        public Dictionary<string, object> Parameters { get; init; } = [];
        public string EventPayload { get; init; }
    }

    public record Result
    {
        public string MessageId { get; init; }
        public string TrackingId { get; init; }
        public DateTime DeliveryTime { get; init; }
    }

    public class Handler(IAppContext appContext,
        IServiceProvider serviceProvider,
        ILogger<Handler> logger) : IRequestHandler<Command, ApiResult<Result>>
    {
        public async Task<ApiResult<Result>> Handle(Command command, CancellationToken cancellationToken)
        {
            var provider = await appContext.MessageProviders
                .IgnoreQueryFilters()
                .Where(c => !c.IsDisabled)
                .Where(c => c.Code == command.ProviderCode)
                .Where(c => c.Id == command.ProviderId)
                .AsNoTracking()
                .FirstOrDefaultAsync(cancellationToken);

            if (provider == null)
                return new FailResult<Result>(ErrorMessages.MESSAGE_PROVIDER_NOT_FOUND, HttpStatusCode.NotFound);

            logger.LogInformation("Verify event payload: {Payload}", command.EventPayload);

            var service = serviceProvider.GetKeyedService<IMessageSender>(provider.Code);
            if (service is null)
                return new FailResult<Result>($"The message sender {provider.Code} is not implemented!", HttpStatusCode.NotImplemented);

            try
            {
                var validationResult = await service.VerifyEventAsync(provider, command.EventPayload, command.Parameters, cancellationToken);
                if (validationResult.IsSuccess)
                {
                    return new SuccessResult<Result>(new Result
                    {
                        MessageId = validationResult.MessageId,
                        TrackingId = validationResult.TrackingId,
                        DeliveryTime = validationResult.DeliveryTime
                    });
                }

                return new FailResult<Result>($"Invalid event payload!", HttpStatusCode.BadRequest);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Verify {Provider} event failed!", command.ProviderCode);
                return new FailResult<Result>($"Invalid event payload!", HttpStatusCode.BadRequest);
            }
        }
    }
}