namespace Auth.Application.EventHandlers.OnActivityLogCreated;

internal class CreateActivityLogEventHandler(IAppContext context) : INotificationHandler<ActivityLogEventItem>
{
    public async Task Handle(ActivityLogEventItem notification,
        CancellationToken cancellationToken)
    {
        if (notification.LogEntities == null || !notification.LogEntities.Any())
            return;

        var activity = new LogActivity
        {
            Time = notification.CreatedDate,
            Label = notification.Label,
            UserId = notification.CurrentUserId,
            Username = notification.CurrentUser,
            Description = notification.Description,
            RequestId = notification.RequestId,
            MethodName = notification.MethodName ?? "N/A",
            Action = notification.Action ?? "N/A",
            Source = notification.Source ?? "N/A",
            IpAddress = notification.IpAddress ?? "N/A"
        };
        context.LogActivities.Add(activity);

        foreach (var entity in notification.LogEntities)
        {
            context.LogEntities.Add(new LogEntity
            {
                ActivityId = activity.Id,
                Time = entity.ActionTime,
                CRUD = Convert.ToChar(entity.Action.ToDescription()),
                Pk = entity.Pk,
                EntityName = entity.EntityName,
                Description = entity.Description,
                OldValue = entity.OldValue.ToJson(),
                NewValue = entity.NewValue.ToJson()
            });
        }
        if (notification.LogRelatedEntities?.Any() == true)
        {
            foreach (var entity in notification.LogRelatedEntities)
            {
                context.LogRelatedEntities.Add(new LogRelatedEntity
                {
                    ActivityId = activity.Id,
                    Pk = entity.Pk,
                    EntityName = entity.EntityName,
                    Description = entity.Description
                });
            }
        }

        await context.SaveChangesAsync(cancellationToken);
    }
}