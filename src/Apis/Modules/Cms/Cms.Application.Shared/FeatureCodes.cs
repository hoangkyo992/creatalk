namespace Cms.Application.Shared;

public static class FeatureCodes
{
    public static class Cms
    {
        public const string Tickets = $"{nameof(Cms)}.{nameof(Tickets)}";
        public const string Attendees = $"{nameof(Cms)}.{nameof(Attendees)}";
        public const string MessageProviders = $"{nameof(Cms)}.{nameof(MessageProviders)}";
    }
}