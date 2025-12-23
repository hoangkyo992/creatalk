

namespace Auth.Application.Shared.Events;

public class AuthEvents
{
    public class OnSessionExpiredEvent : ApplicationEvent
    {
        public string Actor { get; init; }
        public IEnumerable<long> SessionIds { get; init; }
    }
}