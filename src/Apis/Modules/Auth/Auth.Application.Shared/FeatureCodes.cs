namespace Auth.Application.Shared;

public static class FeatureCodes
{
    public static class Administration
    {
        public const string Users = $"{nameof(Administration)}.{nameof(Users)}";
        public const string Roles = $"{nameof(Administration)}.{nameof(Roles)}";
        public const string UserSessions = $"{nameof(Administration)}.{nameof(UserSessions)}";
        public const string Activities = $"{nameof(Administration)}.{nameof(Activities)}";
        public const string Settings = $"{nameof(Administration)}.{nameof(Settings)}";
    }
}