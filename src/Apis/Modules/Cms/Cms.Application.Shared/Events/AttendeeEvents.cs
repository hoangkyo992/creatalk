namespace Cms.Application.Shared.Events;

public class OnAttendeeCreatedEvent : ApplicationEvent
{
    public IEnumerable<long> Ids { get; set; }
}

public class OnAttendeeUpdatedEvent : ApplicationEvent
{
    public IEnumerable<long> Ids { get; set; }
}

public class OnAttendeeDeletedEvent : ApplicationEvent
{
    public IEnumerable<long> Ids { get; set; }
}