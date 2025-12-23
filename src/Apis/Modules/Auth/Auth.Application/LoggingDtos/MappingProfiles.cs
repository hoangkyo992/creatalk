namespace Auth.Application.LoggingDtos;

public class MappingProfiles : Profile
{
    public MappingProfiles()
    {
        CreateMap<User, UserLoggingDto>()
            .ForMember(dest => dest.RoleIds, opt => opt.MapFrom(src => src.Roles.Where(r => !r.IsDeleted).Select(r => r.RoleId)));
        CreateMap<Role, RoleLoggingDto>();
        CreateMap<RoleFeature, RoleFeatureLoggingDto>();
        CreateMap<Setting, SettingLoggingDto>();
    }
}