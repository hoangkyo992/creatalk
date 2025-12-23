namespace Auth.Application.LoggingDtos;

public enum LogLabel
{
    CreateUser = 1,
    UpdateUser = 2,
    DeleteUser = 3,
    ResetUserPassword = 4,
    UpdateUserLanguages = 5,

    CreateUserRole = 6,
    UpdateUserRole = 7,
    DeleteUserRole = 8,
    UpdateUserRoleFeatures = 9,

    CreateSetting = 10,
    UpdateSetting = 11,
    DeleteSetting = 12,
}