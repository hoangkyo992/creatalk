namespace Common.Application.Common.Extensions;

public static class ApplicationEventExtensions
{
    public static ApplicationEvent SetCurrentUser(this ApplicationEvent evt, ICurrentUser currentUser)
    {
        return evt.SetCurrentUser(currentUser.Username, currentUser.Id);
    }
}