using Auth.Application.Features.Credentials;
using Common.Application.Services;
using MediatR;

namespace Auth.Infrastructure.Services;

public class CredentialService(IMediator mediator) : ICredentialService
{
    public async Task<string> GetAsync(string key, CancellationToken cancellationToken)
    {
        try
        {
            var apiResponse = await mediator.Send(new GetByKey.Request
            {
                Key = key
            }, cancellationToken);
            if (apiResponse.IsSuccess && apiResponse.Result != null)
                return apiResponse.Result!.Value;
            return string.Empty;
        }
        catch
        {
            return string.Empty;
        }
    }

    public async Task SetAsync(string key, string value, CancellationToken cancellationToken)
    {
        await mediator.Send(new CreateOrUpdate.Command
        {
            Key = key,
            Value = value
        }, cancellationToken);
    }
}