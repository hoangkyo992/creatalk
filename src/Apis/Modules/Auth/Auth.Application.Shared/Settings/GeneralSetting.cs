namespace Auth.Application.Shared.Settings;

public class GeneralSetting : ISettingObject
{
    /// <summary>
    /// Site title
    /// </summary>
    public string SiteTitle { get; set; } = string.Empty;

    /// <summary>
    /// Tag line
    /// </summary>
    public string Tagline { get; set; } = string.Empty;

    /// <summary>
    /// Site icon
    /// </summary>
    public string SiteIcon { get; set; } = string.Empty;

    /// <summary>
    /// Site logo
    /// </summary>
    public string SiteLogo { get; set; } = string.Empty;

    /// <summary>
    /// Google analytics tracking Id
    /// </summary>
    public string GA4Id { get; set; } = string.Empty;
}