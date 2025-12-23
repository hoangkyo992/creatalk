using AutoMapper;

namespace Cdn.Application.LoggingDtos;

public class MappingProfiles : Profile
{
    public MappingProfiles()
    {
        CreateMap<Album, AlbumLoggingDto>();
        CreateMap<Folder, FolderLoggingDto>();
    }
}