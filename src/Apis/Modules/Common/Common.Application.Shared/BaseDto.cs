using System.Text.Json.Serialization;

namespace Common.Application.Shared;

public abstract class IdDto
{
    [JsonPropertyOrder(int.MinValue)]
    [JsonConverter(typeof(ZCodeJsonConverter))]
    public long Id { get; set; }
}

public abstract class BaseDto : IdDto
{
    public string CreatedBy { get; set; }

    public DateTime CreatedTime { get; set; }

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string? UpdatedBy { get; set; }

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public DateTime? UpdatedTime { get; set; }
}