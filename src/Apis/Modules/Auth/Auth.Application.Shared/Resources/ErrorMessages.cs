namespace Auth.Application.Shared.Resources;

public partial struct ErrorMessages
{
    public const string AUTH_INVALID_PASSWORD = nameof(AUTH_INVALID_PASSWORD);
    public const string AUTH_INVALID_PASSWORD_AND_LOCKED = nameof(AUTH_INVALID_PASSWORD_AND_LOCKED);
    public const string AUTH_USER_LOCKED = nameof(AUTH_USER_LOCKED);
    public const string AUTH_USER_DISABLED = nameof(AUTH_USER_DISABLED);
    public const string AUTH_USER_INACTIVE = nameof(AUTH_USER_INACTIVE);
    public const string AUTH_USER_LOCKED_DUETO_MAX_ATTEMPT = nameof(AUTH_USER_LOCKED_DUETO_MAX_ATTEMPT);
    public const string AUTH_NOT_FOUND = nameof(AUTH_NOT_FOUND);
    public const string AUTH_PASSWORD_TOO_SHORT = nameof(AUTH_PASSWORD_TOO_SHORT);
    public const string AUTH_PASSWORD_NOT_MATCH = nameof(AUTH_PASSWORD_NOT_MATCH);
    public const string AUTH_REFRESH_TOKEN_NOT_FOUND = nameof(AUTH_REFRESH_TOKEN_NOT_FOUND);
    public const string AUTH_REFRESH_TOKEN_EXPIRED = nameof(AUTH_REFRESH_TOKEN_EXPIRED);

    public const string USER_NOT_FOUND = nameof(USER_NOT_FOUND);
    public const string USER_EXISTED = nameof(USER_EXISTED);
    public const string USER_USERNAME_INVALID = nameof(USER_USERNAME_INVALID);

    public const string ROLE_NOT_FOUND = nameof(ROLE_NOT_FOUND);
    public const string ROLE_EXISTED = nameof(ROLE_EXISTED);
    public const string ROLE_HAS_USERS = nameof(ROLE_HAS_USERS);

    public const string SETTING_NOT_FOUND = nameof(SETTING_NOT_FOUND);
    public const string SETTING_EXISTED = nameof(SETTING_EXISTED);
    public const string SETTING_VALUE_INVALID = nameof(SETTING_VALUE_INVALID);
}