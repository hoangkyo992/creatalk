namespace Cdn.Application.Shared.Events;

public class OnFileDeletedEvent : ApplicationEvent
{
    public IEnumerable<long> FileIds { get; init; }
    public long FolderId { get; init; }
    public string FolderName { get; init; }
}

public class OnFileUploadedEvent : ApplicationEvent
{
    public IEnumerable<long> FileIds { get; init; }
    public long FolderId { get; init; }
    public string FolderName { get; init; }
}

public class OnFileMovedEvent : ApplicationEvent
{
    public IEnumerable<long> FileIds { get; init; }
    public long FolderId { get; init; }
    public string FolderName { get; init; }
}

public class OnFileRenamedEvent : ApplicationEvent
{
    public IEnumerable<long> FileIds { get; init; }
    public long FolderId { get; init; }
    public string FolderName { get; init; }
}

public class OnFileMoveToTrashEvent : ApplicationEvent
{
    public List<long> Ids { get; init; }
}

public class OnFileRestoreFromTrashEvent : ApplicationEvent
{
    public List<long> Ids { get; init; }
}