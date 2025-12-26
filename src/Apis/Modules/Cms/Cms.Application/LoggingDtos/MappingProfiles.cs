namespace Cms.Application.LoggingDtos;

public class MappingProfiles : Profile
{
    public MappingProfiles()
    {
        CreateMap<Attendee, AttendeeLoggingDto>();
        CreateMap<MessageProvider, MessageProviderLoggingDto>();
    }
}