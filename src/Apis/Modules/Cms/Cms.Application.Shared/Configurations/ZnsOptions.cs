namespace Cms.Application.Shared.Configurations;

public class ZnsOptions
{
    public const string Section = "Zns";

    public string ServiceUrl { get; init; } = "https://business.openapi.zalo.me";
    public string RefreshTokenUrl { get; init; } = "https://oauth.zaloapp.com/v4/oa/access_token";
    public string RefreshToken { get; init; }
    public string SecretKey { get; init; }
    public string AppId { get; init; }
    public string OAId { get; init; }
    public string Mode { get; init; } = "development";
    public string TemplateId { get; init; }

    public bool IsValid() => !string.IsNullOrWhiteSpace(ServiceUrl)
        && !string.IsNullOrWhiteSpace(SecretKey)
        && !string.IsNullOrWhiteSpace(RefreshTokenUrl)
        && !string.IsNullOrWhiteSpace(RefreshToken)
        && !string.IsNullOrWhiteSpace(AppId)
        && !string.IsNullOrWhiteSpace(Mode)
        && !string.IsNullOrWhiteSpace(TemplateId);
}