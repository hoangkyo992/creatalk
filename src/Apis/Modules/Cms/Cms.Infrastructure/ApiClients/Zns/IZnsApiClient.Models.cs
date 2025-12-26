using System.Text.Json.Serialization;
using HungHd.Shared.Utilities;

namespace Cms.Infrastructure.ApiClients.Zns;

public class SendMessageReqDto
{
    [JsonPropertyName("phone")]
    public string Phone { get; init; }

    [JsonPropertyName("template_id")]
    public string TemplateId { get; init; }

    [JsonPropertyName("template_data")]
    public object TemplateData { get; init; }

    [JsonPropertyName("tracking_id")]
    public string TrackingId { get; init; }
}

public class SendMessageResDto
{
    [JsonPropertyName("error")]
    public int Error { get; init; }

    [JsonPropertyName("message")]
    public string Message { get; init; }

    [JsonPropertyName("data")]
    public SendMessageResDataDto Data { get; init; }

    public override string ToString()
    {
        return this.ToJson();
    }
}

public class SendMessageResDataDto
{
    [JsonPropertyName("sent_time")]
    public string SentTime { get; init; }

    [JsonPropertyName("quota")]
    public SendMessageQuotaDto Quota { get; init; }

    [JsonPropertyName("msg_id")]
    public string MsgId { get; init; }
}

public class SendMessageQuotaDto
{
    [JsonPropertyName("remainingQuota")]
    public string RemainingQuota { get; init; }

    [JsonPropertyName("dailyQuota")]
    public string DailyQuota { get; init; }
}

public class ZnsWebhookReqDto
{
    [JsonPropertyName("event_name")]
    public string EventName { get; init; }

    [JsonPropertyName("message")]
    public ZnsWebhookMessageReqDto Message { get; init; }

    [JsonPropertyName("app_id")]
    public string AppId { get; init; }

    [JsonPropertyName("timestamp")]
    public string Timestamp { get; init; }
}

public class ZnsWebhookMessageReqDto
{
    [JsonPropertyName("delivery_time")]
    public string DeliveryTime { get; init; }

    [JsonPropertyName("msg_id")]
    public string MsgId { get; init; }

    [JsonPropertyName("tracking_id")]
    public string TrackingId { get; init; }
}