using Common.Api.Configurations;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Primitives;

namespace Common.Api.Middleware;

public class AcceptLanguageMiddleware(RequestDelegate next)
{
    private const string LanguageHeaderKey = "Lang";
    private readonly RequestDelegate _next = next;

    public async Task Invoke(HttpContext context,
        IOptions<LanguageConfigurations> languageConfigurationOptions)
    {
        context.Request.Headers.TryGetValue(LanguageHeaderKey, out StringValues langValues);
        var lang = langValues.FirstOrDefault()?.Split(",").FirstOrDefault();
        if (string.IsNullOrWhiteSpace(lang) || !languageConfigurationOptions.Value.GetSupportedCultureInfos().ToList().Exists(c => c.Name.Equals(lang)))
        {
            lang = languageConfigurationOptions.Value.DefaultLanguage;
        }

        var cultureInfo = new System.Globalization.CultureInfo(lang);
        Thread.CurrentThread.CurrentCulture = cultureInfo;
        Thread.CurrentThread.CurrentUICulture = cultureInfo;

        await _next.Invoke(context);
    }
}