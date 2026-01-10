namespace Common.Infrastructure.ApiClients;

public interface IApiTokenResolver
{
    Task ClearAccessToken();

    Task<string> GetAccessToken();

    Dictionary<string, string> AdditionalHeaders { get; }

    string AccessTokenHeaderName { get; }
}