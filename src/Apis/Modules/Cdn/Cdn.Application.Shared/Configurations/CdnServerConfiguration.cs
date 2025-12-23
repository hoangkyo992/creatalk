namespace Cdn.Application.Shared.Configurations;

public class CdnServerConfiguration
{
    public const string ConfigSection = "CdnServer";

    public string Name { get; init; }
    public string ProviderName { get; init; }
    public string PublicPath { get; init; }
    public int Expiration { get; init; }
}