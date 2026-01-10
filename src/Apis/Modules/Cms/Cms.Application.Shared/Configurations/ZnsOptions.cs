namespace Cms.Application.Shared.Configurations;

public class ZnsOptions
{
    public const string Section = "Zns";

    public string ServiceUrl { get; init; } = "https://business.openapi.zalo.me";
    public string RefreshTokenUrl { get; init; } = "https://oauth.zaloapp.com/v4/oa/access_token";
    public string SendZnsEndpoint { get; init; } = "message/template";
    public string RefreshToken { get; init; }
    public string AccessToken { get; init; }
    public string SecretKey { get; init; }
    public string OASecretKey { get; init; }
    public string AppId { get; init; }
    public string OAId { get; init; }
    public string Mode { get; init; } = "development";
    public string TemplateId { get; init; }
    public string Provider { get; init; } = "Zalo";
    public string AccessTokenHeaderName { get; init; } = "access_token";

    public bool IsValid() => !string.IsNullOrWhiteSpace(ServiceUrl)
        && !string.IsNullOrWhiteSpace(SendZnsEndpoint)
        && !string.IsNullOrWhiteSpace(RefreshTokenUrl)
        && !string.IsNullOrWhiteSpace(AppId)
        && !string.IsNullOrWhiteSpace(OAId)
        && !string.IsNullOrWhiteSpace(TemplateId);
}