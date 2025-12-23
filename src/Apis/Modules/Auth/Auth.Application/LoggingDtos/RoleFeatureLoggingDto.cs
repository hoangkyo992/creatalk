namespace Auth.Application.LoggingDtos;

public class RoleFeatureLoggingDto
{
    [JsonConverter(typeof(ZCodeJsonConverter))]
    public long RoleId { get; init; }

    public string RoleName { get; set; }
    public string Feature { get; init; }
    public string Action { get; init; }
}