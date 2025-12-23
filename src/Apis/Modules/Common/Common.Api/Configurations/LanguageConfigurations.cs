using System.Globalization;

namespace Common.Api.Configurations;

public class LanguageConfigurations
{
    public string DefaultLanguage { get; set; }
    public string SupportedLanguages { get; set; }

    public CultureInfo[] GetSupportedCultureInfos()
    {
        return SupportedLanguages.Split(";").Select(c => new CultureInfo(c)).ToArray();
    }
}