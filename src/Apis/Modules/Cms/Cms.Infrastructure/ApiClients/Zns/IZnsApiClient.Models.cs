namespace Cms.Infrastructure.ApiClients.Zns;

public class SendMessageReqDto
{
    [JsonPropertyName("mode")]
    public string Mode { get; init; }

    [JsonPropertyName("phone")]
    public string Phone { get; init; }

    [JsonPropertyName("template_id")]
    public string TemplateId { get; init; }

    [JsonPropertyName("oa_id")]
    public string OAId { get; init; }

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

    [JsonPropertyName("app_id")]
    public string AppId { get; init; }

    [JsonPropertyName("timestamp")]
    public string Timestamp { get; init; }
}

public class ZnsWebhookReqDto<T>
{
    [JsonPropertyName("event_name")]
    public string EventName { get; init; }

    [JsonPropertyName("message")]
    public T Message { get; init; }

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

public class ZnsRefreshTokenResDto
{
    [JsonPropertyName("error_name")]
    public string ErrorName { get; init; }

    [JsonPropertyName("error_reason")]
    public string ErrorReason { get; init; }

    [JsonPropertyName("error_description")]
    public string ErrorDescription { get; init; }

    [JsonPropertyName("access_token")]
    public string AccessToken { get; init; }

    [JsonPropertyName("refresh_token")]
    public string RefreshToken { get; init; }

    [JsonPropertyName("expires_in")]
    public string ExpiresIn { get; init; }

    [JsonIgnore]
    public DateTime ExpirationTime
    {
        get
        {
            try
            {
                return DateTime.UtcNow.AddSeconds(Convert.ToInt32(this.ExpiresIn));
            }
            catch
            {
                return DateTime.UtcNow;
            }
        }
    }
}