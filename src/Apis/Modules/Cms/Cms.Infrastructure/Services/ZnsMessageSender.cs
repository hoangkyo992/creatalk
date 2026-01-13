using System.Security.Cryptography;
using System.Text;
using Cms.Application.Services.Abstractions;
using Cms.Domain.Entities;
using Cms.Infrastructure.ApiClients.Zns;

namespace Cms.Infrastructure.Services;

public class ZnsMessageSender(IZnsApiClient znsClient, ILogger<ZnsMessageSender> logger, IOptions<ZnsOptions> options) : IMessageSender
{
    public virtual async Task<SendMessageResponse> SendAsync(
        MessageProvider provider,
        AttendeeMessage message,
        string templateCode,
        CancellationToken cancellationToken = default)
    {
        var payload = new SendMessageReqDto
        {
            Mode = options.Value.Mode,
            Phone = message.Attendee.PhoneNumber,
            TemplateId = options.Value.TemplateId,
            OAId = options.Value.OAId,
            TrackingId = ZCode.Get(message.Id),
            TemplateData = GetTemplateData(templateCode, message)
        };
        var response = await znsClient.SendMessageAsync(options.Value.SendZnsEndpoint, payload, cancellationToken);
        if (!response.IsSuccessStatusCode)
        {
            return new SendMessageResponse
            {
                IsSuccess = response.IsSuccessStatusCode,
                RequestPayload = payload.ToJson(),
                ResponsePayload = await response.Content.ReadAsStringAsync(cancellationToken),
            };
        }

        var result = new SendMessageResponse
        {
            RequestPayload = payload.ToJson(),
            ResponsePayload = await response.Content.ReadAsStringAsync(cancellationToken)
        };

        if (options.Value.Provider == "SimpleZns")
        {
            var resDto = await response.Content.ReadAsAsync<SendSimpleMessageResDto>();
            result.IsSuccess = resDto.Status == "success" && resDto.Data.Error == 0;
            result.ErrorMessage = resDto.Data.Message;
            result.MessageId = resDto.Data.Data?.MsgId ?? string.Empty;
        }
        else
        {
            var resDto = await response.Content.ReadAsAsync<SendMessageResDto>();
            result.IsSuccess = resDto.Error == 0;
            result.ErrorMessage = resDto.Message;
            result.MessageId = resDto.Data?.MsgId ?? string.Empty;
        }

        return result;
    }

    public virtual async Task<VerifyEventResponse> VerifyEventAsync(MessageProvider provider, string payload, Dictionary<string, object> parameters, CancellationToken cancellationToken)
    {
        try
        {
            var znsObj = payload.FromJson<ZnsWebhookReqDto>();
            _ = long.TryParse(znsObj.Timestamp, System.Globalization.NumberStyles.None, null, out var timestamp);

            // Reconstruct the signature base string: sha256(appId + data + timeStamp + OAsecretKey)
            var signatureBase = $"{options.Value.AppId}{payload}{timestamp}{options.Value.OASecretKey}";

            // Compute the HMAC-SHA256 signature
            var computedSignature = CalculateHmacSha256(signatureBase);
            computedSignature = $"mac={computedSignature}";
            // Compare the computed signature with the received signature (case-insensitive)
            var isValid = string.Equals(computedSignature, parameters.GetValueOrDefault("Signature")?.ToString(), StringComparison.OrdinalIgnoreCase);
            if (isValid)
            {
                return await Task.FromResult(new VerifyEventResponse
                {
                    IsSuccess = true,
                    EventName = znsObj.EventName,
                });
            }
            return new VerifyEventResponse
            {
                IsSuccess = false,
                ErrorMessage = "Invalid signature"
            };
        }
        catch (Exception ex)
        {
            return new VerifyEventResponse
            {
                IsSuccess = false,
                ErrorMessage = ex.Message
            };
        }
    }

    public virtual async Task<Dictionary<string, object?>> GetEventDataAsync(string eventName, string payload, CancellationToken cancellationToken)
    {
        try
        {
            if (eventName == "user_received_message")
            {
                var msg = payload.FromJson<ZnsWebhookReqDto<ZnsWebhookMessageReqDto>>()!.Message;
                _ = long.TryParse(msg.DeliveryTime, System.Globalization.NumberStyles.None, null, out var deliveryTime);
                var time = DateTimeOffset.FromUnixTimeMilliseconds(deliveryTime);

                return await Task.FromResult(new Dictionary<string, object?>
                {
                    { "DeliveryTime", time.DateTime.ToLocalTime() },
                    { "MessageId", msg.MsgId },
                    { "TrackingId", msg.TrackingId }
                });
            }
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Failed to parse event data of event {EventName}", eventName);
        }

        return [];
    }

    private static string CalculateHmacSha256(string data)
    {
        using SHA256 sha256 = SHA256.Create();
        byte[] bytes = Encoding.UTF8.GetBytes(data);
        byte[] hash = sha256.ComputeHash(bytes);

        // Convert to hex string
        StringBuilder sb = new StringBuilder();
        foreach (byte b in hash)
        {
            sb.Append(b.ToString("x2"));
        }
        return sb.ToString();
    }

    private static object GetTemplateData(string template, AttendeeMessage message)
    {
        switch (template)
        {
            case "TETKHOISAC":
                return new
                {
                    name = message.Attendee.PhoneNumber,
                    ma = message.Attendee.TicketNumber,
                    ticket_url = message.Attendee.TicketPath
                };

            default:
                return new
                {
                };
        }
    }
}