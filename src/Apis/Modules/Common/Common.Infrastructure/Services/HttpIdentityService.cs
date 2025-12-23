using Common.Application.Services;

namespace Common.Infrastructure.Services;

public class HttpIdentityService : IIdentityService
{
    private readonly HttpClient _httpClient;
    private readonly ICurrentUser? _currentUser;
    private readonly ILogger<HttpIdentityService> _logger;

    public HttpIdentityService(IOptions<ApplicationOptions> applicationOptions,
        ICurrentUser? currentUser,
        ILogger<HttpIdentityService> logger)
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

    public async Task<ApplicationIdentity> GetIdentity(string token)
    {
        try
        {
            var headers = new Dictionary<string, string>()
            {
                { "Authorization", $"Bearer {token}" }
            };
            return await _httpClient.GetAsync(headers, $"api/auth/me")
                .RetrieveContent<ApplicationIdentity>();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, nameof(GetIdentity));
            return new ApplicationIdentity();
        }
    }

    public async Task<bool> IsAccessable(List<(string Name, string Action)> features)
    {
        try
        {
            var headers = CreateHeaders(_currentUser);
            return await _httpClient.PostAsJsonAsync(headers, "api/auth/check-access", new
            {
                features
            }).RetrieveContent<bool>();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, nameof(IsAccessable));
            return false;
        }
    }

    private static Dictionary<string, string> CreateHeaders(ICurrentUser? currentUser)
    {
        return new Dictionary<string, string>()
        {
            { "Authorization", $"Bearer {currentUser?.AccessToken ?? string.Empty}" },
        };
    }
}