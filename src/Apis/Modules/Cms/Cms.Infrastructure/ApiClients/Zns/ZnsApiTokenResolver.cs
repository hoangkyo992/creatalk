using Common.Application.Services;

namespace Cms.Infrastructure.ApiClients.Zns;

internal class ZnsApiTokenResolver(IHttpClientFactory httpClientFactory,
    ILogger<ZnsApiTokenResolver> logger,
    IServiceProvider serviceProvider,
    ICryptographyService cryptographyService,
    IOptions<ZnsOptions> znsOptions) : IApiTokenResolver
{
    private static readonly Lock ObjLock = new();
    private readonly ZnsOptions _znsOptions = znsOptions.Value;
    private DateTime? _expiresAt;
    private string? _accessToken;

    public Dictionary<string, string> AdditionalHeaders => [];

    private async Task RequestTokenAsync()
    {
        if (string.IsNullOrEmpty(_accessToken) || !_expiresAt.HasValue || DateTime.UtcNow >= _expiresAt.Value)
        {
            using var scope = serviceProvider.CreateScope();
            var credentialService = scope.ServiceProvider.GetRequiredService<ICredentialService>();

            var accessKey = $"ZNS_ACT_{_znsOptions.AppId}_{_znsOptions.OAId}";
            var accessToken = await credentialService.GetAsync(accessKey, CancellationToken.None);
            if (!string.IsNullOrWhiteSpace(accessToken))
            {
                _expiresAt = DateTime.UtcNow.AddHours(25) - TimeSpan.FromMinutes(5);
                _accessToken = cryptographyService.Decrypt(accessToken);
                return;
            }

            var refreshKey = $"ZNS_RFT_{_znsOptions.AppId}_{_znsOptions.OAId}";
            var refreshToken = await credentialService.GetAsync(refreshKey, CancellationToken.None);
            if (string.IsNullOrWhiteSpace(refreshToken))
            {
                refreshToken = _znsOptions.RefreshToken;
            }
            else
            {
                refreshToken = cryptographyService.Decrypt(refreshToken);
            }

            List<KeyValuePair<string, string>> payload =
            [
                new KeyValuePair<string, string>("app_id", _znsOptions.AppId),
                new KeyValuePair<string, string>("grant_type", "refresh_token"),
                new KeyValuePair<string, string>("refresh_token", refreshToken)
            ];
            var content = new FormUrlEncodedContent(payload);

            using var client = httpClientFactory.CreateClient("ZNS");
            client.DefaultRequestHeaders.Remove("secret_key");
            client.DefaultRequestHeaders.Add("secret_key", _znsOptions.SecretKey);

            var response = await client.PostAsync(_znsOptions.RefreshTokenUrl, content).RetrieveContent<ZnsRefreshTokenResDto>();
            if (!string.IsNullOrWhiteSpace(response.ErrorName))
            {
                logger.LogError("Failed to get refresh token. Response: {Response}", response.ToJson());
                throw new InvalidOperationException($"Failed to get refresh token. Response: {response.ToJson()}");
            }

            _expiresAt = response.ExpirationTime - TimeSpan.FromMinutes(5);
            _accessToken = response.AccessToken;

            await credentialService.SetAsync(accessKey, cryptographyService.Encrypt(response.AccessToken), CancellationToken.None);
            await credentialService.SetAsync(refreshKey, cryptographyService.Encrypt(response.RefreshToken), CancellationToken.None);
        }
    }

    public async Task<string> GetAccessToken()
    {
        if (string.IsNullOrWhiteSpace(_accessToken))
        {
            lock (ObjLock)
            {
                RequestTokenAsync().Wait();
            }
        }
        return await Task.FromResult(_accessToken ?? string.Empty);
    }

    public async Task ClearAccessToken()
    {
        _accessToken = null;

        using var scope = serviceProvider.CreateScope();
        var credentialService = serviceProvider.GetRequiredService<ICredentialService>();
        var key = $"ZNS_ACT_{_znsOptions.AppId}_{_znsOptions.OAId}";
        await credentialService.SetAsync(key, "", CancellationToken.None);
    }
}