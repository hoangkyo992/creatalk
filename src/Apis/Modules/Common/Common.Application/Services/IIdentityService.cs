namespace Common.Application.Services;

public interface IIdentityService
{
    Task<ApplicationIdentity> GetIdentity(string token);

    Task<bool> IsAccessable(List<(string Name, string Action)> features);
}