using Common.Application.Services;
using Microsoft.Extensions.Configuration;

namespace Common.Infrastructure.Services;

public class HttpCdnService : ICdnService
{
    private readonly HttpClient _httpClient;
    private readonly ICurrentUser? _currentUser;
    private readonly ILogger<HttpCdnService> _logger;

    public HttpCdnService(IConfiguration configuration,
        ICurrentUser? currentUser,
        ILogger<HttpCdnService> logger)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        string baseUrl = configuration.GetValue("CdnServer", "ServerUrl");
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

    public async Task<FileItem?> GetFileAsync(long id, CancellationToken cancellationToken = default)
    {
        try
        {
            var headers = CreateHeaders(_currentUser);
            var response = await _httpClient.GetAsync(headers, $"api/files/{id}").RetrieveContent<ApiResult<FileItem>>();
            return response.Result;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, nameof(GetFileAsync));
            return null;
        }
    }
}