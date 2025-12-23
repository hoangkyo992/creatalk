using Common.Domain.Interfaces;

namespace Common.Application.Common.Extensions;

public static class AppSettingExtensions
{
    public static bool IsTrue<T>(this IEnumerable<T> settings, string key) where T : ISettingEntity
    {
        return bool.TryParse(settings.SingleOrDefault(x => x.Key == key)?.Value?.ToLower(), out var result) && result;
    }

    public static string GetValueOrDefault<T>(this IEnumerable<T> settings, string key, string defaltValue) where T : ISettingEntity
    {
        return settings.Where(x => x.Key == key).Select(c => c.Value).SingleOrDefault() ?? defaltValue;
    }

    public static string? GetValue<T>(this IEnumerable<T> settings, string key) where T : ISettingEntity
    {
        return settings.Where(x => x.Key == key).Select(c => c.Value).SingleOrDefault();
    }

    public static string[] GetValues<T>(this IEnumerable<T> settings, string key) where T : ISettingEntity
    {
        return [.. settings.Where(x => x.Key == key).Select(c => c.Value)];
    }
}