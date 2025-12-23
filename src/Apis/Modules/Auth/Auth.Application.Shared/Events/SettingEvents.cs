namespace Auth.Application.Shared.Events;

public class SettingEvents
{
    public class OnDataCreatedEvent : ApplicationEvent
    {
        public IEnumerable<long> Ids { get; init; }
    }

    public class OnDataUpdatedEvent : ApplicationEvent
    {
        public IEnumerable<long> Ids { get; init; }
    }

    public class OnDataDeletedEvent : ApplicationEvent
    {
        public IEnumerable<long> Ids { get; init; }
    }
}