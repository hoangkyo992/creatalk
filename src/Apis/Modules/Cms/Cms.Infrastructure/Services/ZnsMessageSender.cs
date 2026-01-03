using System.Security.Cryptography;
using System.Text;
using Cms.Application.Services.Abstractions;
using Cms.Domain.Entities;
using Cms.Infrastructure.ApiClients.Zns;

namespace Cms.Infrastructure.Services;

public class ZnsMessageSender(IZnsApiClient znsClient, IOptions<ZnsOptions> options) : IMessageSender
{
    public async Task<SendMessageResponse> SendAsync(MessageProvider provider, AttendeeMessage message, CancellationToken cancellationToken = default)
    {
        var payload = new SendMessageReqDto
        {
            Mode = options.Value.Mode,
            Phone = message.Attendee.PhoneNumber,
            TemplateId = options.Value.TemplateId,
            TrackingId = ZCode.Get(message.Id),
            TemplateData = new
            {
                LINK = message.Attendee.TicketId
            }
        };
        var response = await znsClient.SendMessageAsync(payload, cancellationToken);
        if (!response.IsSuccessStatusCode)
        {
            return new SendMessageResponse
            {
                IsSuccess = response.IsSuccessStatusCode,
                RequestPayload = payload.ToJson(),
                ResponsePayload = await response.Content.ReadAsStringAsync(cancellationToken),
            };
        }

        var resDto = await response.Content.ReadAsAsync<SendMessageResDto>();
        return new SendMessageResponse
        {
            IsSuccess = resDto.Error == 0,
            ErrorMessage = resDto.Message,
            MessageId = resDto.Data?.MsgId ?? string.Empty,
            RequestPayload = payload.ToJson(),
            ResponsePayload = await response.Content.ReadAsStringAsync(cancellationToken)
        };
    }

    public async Task<VerifyEventResponse> VerifyEventAsync(MessageProvider provider, string payload, Dictionary<string, object> parameters, CancellationToken cancellationToken)
    {
        try
        {
            var znsObj = payload.FromJson<ZnsWebhookReqDto>();
            _ = long.TryParse(znsObj.Message.DeliveryTime, System.Globalization.NumberStyles.None, null, out var timestamp);

            // Reconstruct the signature base string: sha256(appId + data + timeStamp + OAsecretKey)
            var signatureBase = $"{options.Value.AppId}{payload}{timestamp}{options.Value.SecretKey}";

            // Compute the HMAC-SHA256 signature
            var computedSignature = CalculateHmacSha256(signatureBase, options.Value.SecretKey);

            // Compare the computed signature with the received signature (case-insensitive)
            var isValid = string.Equals(computedSignature, parameters.GetValueOrDefault("signature")?.ToString(), StringComparison.OrdinalIgnoreCase);
            if (isValid)
            {
                var time = DateTimeOffset.FromUnixTimeMilliseconds(timestamp);
                return await Task.FromResult(new VerifyEventResponse
                {
                    IsSuccess = false,
                    DeliveryTime = time.ToUniversalTime().DateTime,
                    MessageId = znsObj.Message.MsgId,
                    TrackingId = znsObj.Message.TrackingId,
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

    private static string CalculateHmacSha256(string data, string key)
    {
        var keyBytes = Encoding.UTF8.GetBytes(key);
        var dataBytes = Encoding.UTF8.GetBytes(data);
        using var hmac = new HMACSHA256(keyBytes);
        var hashBytes = hmac.ComputeHash(dataBytes);

        // Zalo expects the signature in lowercase hex string format
        var builder = new StringBuilder();
        foreach (var t in hashBytes)
        {
            builder.Append(t.ToString("x2"));
        }
        return builder.ToString();
    }
}