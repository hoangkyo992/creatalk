using Auth.Application.Features.Settings;
using Common.Application.Services;
using Common.Application.Shared;
using HungHd.Shared.Utilities;
using MediatR;

namespace Auth.Infrastructure.Services;

public class SettingService(IMediator mediator) : ISettingService
{
    public async Task<T> GetAsync<T>(string key, CancellationToken cancellationToken) where T : ISettingObject
    {
        try
        {
            var apiResponse = await mediator.Send(new GetByKey.Request
            {
                Key = key
            }, cancellationToken);
            if (apiResponse.IsSuccess && apiResponse.Result != null)
                return apiResponse.Result!.Value.FromJson<T>();
            return Activator.CreateInstance<T>();
        }
        catch
        {
            return Activator.CreateInstance<T>();
        }
    }
}