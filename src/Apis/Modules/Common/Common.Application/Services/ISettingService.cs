
namespace Common.Application.Services;

public interface ISettingService
{
    Task<T> GetAsync<T>(string key, CancellationToken cancellationToken) where T : ISettingObject;
}