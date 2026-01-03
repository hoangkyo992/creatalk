using Common.Application.Services;

namespace Common.Infrastructure.Services;

public class HttpCredentialService : ICredentialService
{
    private readonly HttpClient _httpClient;
    private readonly ICurrentUser? _currentUser;
    private readonly ILogger<HttpCredentialService> _logger;

    public HttpCredentialService(IOptions<ApplicationOptions> applicationOptions,
        ICurrentUser? currentUser,
        ILogger<HttpCredentialService> logger)
    {
        ArgumentNullException.ThrowIfNull(applicationOptions);
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        string baseUrl = applicationOptions.Value.IdentityServerUrl;
        if (!baseUrl.EndsWith('/'))
            baseUrl += "/";
        _currentUser = currentUser;
        _httpClient = new HttpClient()
        {
            BaseAddress = new Uri(baseUrl),
            Timeout = TimeSpan.FromSeconds(60)
        };
    }

    private static Dictionary<string, string> CreateHeaders(ICurrentUser? currentUser)
    {
        return new Dictionary<string, string>()
        {
            { "Authorization", $"Bearer {currentUser?.AccessToken ?? string.Empty}" },
        };
    }

    public async Task<string> GetAsync(string key, CancellationToken cancellationToken)
    {
        try
        {
            var headers = CreateHeaders(_currentUser);
            var apiResponse = await _httpClient.GetAsync(headers, $"api/credentials/key?key={key}").RetrieveContent<ApiResult<CredentialResDto>>();
            if (apiResponse.IsSuccess)
                return apiResponse.Result.Value.FromJson<CredentialResDto>().Value;
            return string.Empty;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, nameof(GetAsync));
            return string.Empty;
        }
    }

    public async Task SetAsync(string key, string value, CancellationToken cancellationToken)
    {
        var headers = CreateHeaders(_currentUser);
        await _httpClient.PostAsJsonAsync(headers, $"api/credentials", new CredentialResDto
        {
            Key = key,
            Value = value
        });
    }

    public sealed record CredentialResDto
    {
        public string Key { get; init; }
        public string Value { get; init; }
    }
}