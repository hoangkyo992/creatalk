namespace Cms.Infrastructure.ApiClients.Zns;

public class SendSimpleMessageReqDto
{
    [JsonPropertyName("phone")]
    public string Phone { get; init; }

    [JsonPropertyName("template_id")]
    public string TemplateId { get; init; }

    [JsonPropertyName("template_data")]
    public object TemplateData { get; init; }

    [JsonPropertyName("oa_id")]
    public string OAId { get; init; }
}

public class SendSimpleMessageResDto
{
    [JsonPropertyName("status")]
    public string Status { get; init; }

    [JsonPropertyName("data")]
    public SendMessageResDto Data { get; init; }

    public override string ToString()
    {
        return this.ToJson();
    }
}