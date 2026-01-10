namespace Cms.Application.Services.Abstractions;

public interface IMessageSender
{
    Task<VerifyEventResponse> VerifyEventAsync(MessageProvider provider, string payload, Dictionary<string, object> parameters, CancellationToken cancellationToken);

    Task<SendMessageResponse> SendAsync(MessageProvider provider, AttendeeMessage message, string templateCode, CancellationToken cancellationToken = default);
}

public class SendMessageResponse
{
    public string RequestPayload { get; init; }
    public string ResponsePayload { get; init; }
    public virtual bool IsSuccess { get; set; }
    public virtual string ErrorMessage { get; set; }
    public virtual string MessageId { get; set; }
}

public class VerifyEventResponse
{
    public bool IsSuccess { get; init; }
    public string ErrorMessage { get; init; }
    public string MessageId { get; init; }
    public string TrackingId { get; init; }
    public DateTime DeliveryTime { get; init; }
}