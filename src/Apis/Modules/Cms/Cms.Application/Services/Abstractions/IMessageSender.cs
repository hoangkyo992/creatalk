namespace Cms.Application.Services.Abstractions;

public interface IMessageSender
{
    Task<SendMessageResponse> SendAsync(MessageProvider provider, AttendeeMessage message, CancellationToken cancellationToken = default);
}

public class SendMessageResponse
{
    public bool IsSuccess { get; init; }
    public string ErrorMessage { get; init; }
    public string RequestPayload { get; init; }
    public string ResponsePayload { get; init; }
}