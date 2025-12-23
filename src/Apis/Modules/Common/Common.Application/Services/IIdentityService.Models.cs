namespace Common.Application.Services;

public record ApplicationIdentity
{
    [JsonConverter(typeof(ZCodeJsonConverter))]
    public long Id { get; init; }
    [JsonConverter(typeof(ZCodeJsonConverter))]
    public long SessionId { get; init; }
    public string Username { get; init; }
    public string Email { get; init; }
    public string? Phone { get; init; }
    public string? Avatar { get; init; }
    public string DisplayName { get; init; }
    public bool IsRobot { get; init; }
    public bool PasswordChanged { get; init; }
    public bool IsSystemUser { get; init; }
}