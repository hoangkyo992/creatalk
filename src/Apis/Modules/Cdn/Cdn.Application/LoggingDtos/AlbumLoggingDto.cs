namespace Cdn.Application.LoggingDtos;

public class AlbumLoggingDto
{
    public string Name { get; init; }
    public string Description { get; set; }
    public bool IsEmpty { get; set; }
    public bool IsDeleted { get; init; }
}