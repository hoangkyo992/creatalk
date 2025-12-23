namespace Cdn.Application.Shared.Events;

public class OnFolderMovedEvent : ApplicationEvent
{
    public IEnumerable<long> Ids { get; init; }
}

public class OnFolderUpdatedEvent : ApplicationEvent
{
    public List<long> Ids { get; init; }
}

public class OnFolderCreatedEvent : ApplicationEvent
{
    public List<long> Ids { get; init; }
}

public class OnFolderDeletedEvent : ApplicationEvent
{
    public List<long> Ids { get; init; }
}

public class OnFolderRenamedEvent : ApplicationEvent
{
    public List<long> Ids { get; init; }
}

public class OnFolderMoveToTrashEvent : ApplicationEvent
{
    public List<long> Ids { get; init; }
}

public class OnFolderRestoreFromTrashEvent : ApplicationEvent
{
    public List<long> Ids { get; init; }
}