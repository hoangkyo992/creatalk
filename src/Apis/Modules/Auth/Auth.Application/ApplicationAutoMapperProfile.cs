using Auth.Application.Actions;
using Auth.Application.Features.Settings.Dtos;

namespace Auth.Application;

internal class ApplicationAutoMapperProfile : Profile
{
    public ApplicationAutoMapperProfile()
    {
        CreateMap<Setting, SettingResDto>()
            .AfterMap<UpdateSettingValueAction>();
    }
}