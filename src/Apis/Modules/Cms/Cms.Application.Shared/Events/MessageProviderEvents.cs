namespace Cms.Application.Shared.Events;

public class OnMessageProviderCreatedEvent : ApplicationEvent
{
    public IEnumerable<long> Ids { get; set; }
}

public class OnMessageProviderUpdatedEvent : ApplicationEvent
{
    public IEnumerable<long> Ids { get; set; }
}

public class OnMessageProviderDeletedEvent : ApplicationEvent
{
    public IEnumerable<long> Ids { get; set; }
}