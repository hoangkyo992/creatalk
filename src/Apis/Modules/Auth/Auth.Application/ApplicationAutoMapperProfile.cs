using Auth.Application.Actions;
using Auth.Application.Features.Credentials.Dtos;
using Auth.Application.Features.Settings.Dtos;

namespace Auth.Application;

internal class ApplicationAutoMapperProfile : Profile
{
    public ApplicationAutoMapperProfile()
    {
        CreateMap<Credential, CredentialResDto>();
        CreateMap<Setting, SettingResDto>().AfterMap<UpdateSettingValueAction>();
    }
}