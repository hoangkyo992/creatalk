using MediatR;

namespace Common.Application.Shared;

public abstract class ApplicationEvent : INotification
{
    public string CurrentUser { get; private set; }
    public long CurrentUserId { get; private set; }
    public PublishStrategy PublishStrategy { get; set; } = PublishStrategy.Async;

    public ApplicationEvent SetCurrentUser(string currentUser, long currentUserId)
    {
        CurrentUser = currentUser;
        CurrentUserId = currentUserId;
        return this;
    }
}