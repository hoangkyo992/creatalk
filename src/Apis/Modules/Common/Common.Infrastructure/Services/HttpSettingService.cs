using Common.Application.Services;

namespace Common.Infrastructure.Services;

public class HttpSettingService : ISettingService
{
    private readonly HttpClient _httpClient;
    private readonly ICurrentUser? _currentUser;
    private readonly ILogger<HttpSettingService> _logger;

    public HttpSettingService(IOptions<ApplicationOptions> applicationOptions,
        ICurrentUser? currentUser,
        ILogger<HttpSettingService> logger)
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

    public async Task<T> GetAsync<T>(string key, CancellationToken cancellationToken) where T : Application.Shared.ISettingObject
    {
        try
        {
            var headers = CreateHeaders(_currentUser);
            var apiResponse = await _httpClient.GetAsync(headers, $"api/settings/key?key={key}").RetrieveContent<ApiResult<SettingResDto>>();
            if (apiResponse.IsSuccess)
                return apiResponse.Result.Value.FromJson<T>();
            return Activator.CreateInstance<T>();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, nameof(GetAsync));
            return Activator.CreateInstance<T>();
        }
    }

    public sealed record SettingResDto
    {
        public string Key { get; init; }
        public string Value { get; init; }
    }
}