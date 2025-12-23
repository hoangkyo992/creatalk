namespace Cdn.Application.LoggingDtos;

public class FolderLoggingDto
{
    public string Name { get; init; }
    public FolderStatus StatusId { get; init; }
    public long? ParentId { get; init; }
    public bool IsDeleted { get; init; }
}