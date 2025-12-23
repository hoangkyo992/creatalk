namespace Auth.Application.LoggingDtos;

public class UserLoggingDto
{
    public string Username { get; init; }
    public string DisplayName { get; init; }
    public string Email { get; init; }
    public string? Phone { get; init; }
    public UserStatus StatusId { get; init; }
    public string StatusCode => EnumHelper<UserStatus>.GetLocalizedKey(StatusId);
    public IEnumerable<long> RoleIds { get; init; } = [];
}