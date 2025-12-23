namespace Auth.Domain.Shared.Enums;

public enum UserStatus
{
    PendingApproval = 0,
    Active = 1,
    Locked = 9,
    Disabled = 99
}