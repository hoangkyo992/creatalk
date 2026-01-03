namespace Common.Application.Services;

public interface ICredentialService
{
    Task<string> GetAsync(string key, CancellationToken cancellationToken);

    Task SetAsync(string key, string value, CancellationToken cancellationToken);
}