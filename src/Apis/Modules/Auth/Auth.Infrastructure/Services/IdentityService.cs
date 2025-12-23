using Auth.Application.Features.Auth;
using Common.Application.Services;
using MediatR;

namespace Auth.Infrastructure.Services;

public class IdentityService(IMediator mediator) : IIdentityService
{
    public async Task<ApplicationIdentity> GetIdentity(string token)
    {
        try
        {
            var session = await mediator.Send(new GetIdentity.Request(token));
            if (!session.IsSuccess)
                return new ApplicationIdentity();
            return session.Result.AuthData ?? new ApplicationIdentity();
        }
        catch
        {
            return new ApplicationIdentity();
        }
    }

    public async Task<bool> IsAccessable(List<(string Name, string Action)> features)
    {
        try
        {
            var result = await mediator.Send(new CheckAccess.Request
            {
                Features = features.Select(c => new CheckAccess.Request.Feature(c.Name, c.Action)).ToList()
            });
            if (!result.IsSuccess)
                return false;
            return result.Result.IsAccessable;
        }
        catch
        {
            return false;
        }
    }
}