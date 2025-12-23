namespace Cdn.Application.Shared;

public static class FeatureCodes
{
    public static class Cdn
    {
        public const string Albums = $"{nameof(Cdn)}.{nameof(Albums)}";
        public const string Library = $"{nameof(Cdn)}.{nameof(Library)}";
    }
}