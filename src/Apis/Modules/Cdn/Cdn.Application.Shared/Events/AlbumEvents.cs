namespace Cdn.Application.Shared.Events;

public class OnAlbumMovedEvent : ApplicationEvent
{
    public IEnumerable<long> Ids { get; init; }
}

public class OnAlbumUpdatedEvent : ApplicationEvent
{
    public List<long> Ids { get; init; }
}

public class OnAlbumCreatedEvent : ApplicationEvent
{
    public List<long> Ids { get; init; }
}

public class OnAlbumDeletedEvent : ApplicationEvent
{
    public List<long> Ids { get; init; }
}

public class OnAlbumFilesAddedEvent : ApplicationEvent
{
    public long Id { get; init; }
    public List<long> FileIds { get; init; }
}

public class OnAlbumFilesRemovedEvent : ApplicationEvent
{
    public long Id { get; init; }
    public List<long> FileIds { get; init; }
}

public class OnAlbumFilesUpdatedEvent : ApplicationEvent
{
    public long Id { get; init; }
    public List<long> FileIds { get; init; }
}