using Common.Application.Services;

namespace Common.Infrastructure.Services.ActivityLog;

public class ActivityLogService : IActivityLogService
{
    private bool _isCommited = false;
    private readonly IMediator _mediator;
    private readonly ActivityLogEventItem _notification;

    public ActivityLogService(IMediator mediator)
    {
        _mediator = mediator;
        _notification = new ActivityLogEventItem()
        {
            LogEntities = [],
            LogRelatedEntities = [],
            NotifiedUserIds = []
        };
    }

    private void Setup(string label, string description)
    {
        _notification.Label = label;
        _notification.Description = description;
    }

    public IActivityLogService Setup(Enum label,
        string description,
        ICurrentUser currentUser,
        LogRelatedEntityModel? relatedItem = null)
    {
        Setup(label.ToString(), description);
        _notification.SetCurrentUser(currentUser.Username, currentUser.Id);
        if (relatedItem != null)
            AddRelatedEntity(new LogRelatedEntityModel(relatedItem.EntityName, relatedItem.Pk));
        return this;
    }

    public IActivityLogService Setup(string label,
        string description,
        ICurrentUser currentUser,
        LogRelatedEntityModel? relatedItem = null)
    {
        Setup(label, description);
        _notification.SetCurrentUser(currentUser.Username, currentUser.Id);
        if (relatedItem != null)
            AddRelatedEntity(new LogRelatedEntityModel(relatedItem.EntityName, relatedItem.Pk));
        return this;
    }

    public IActivityLogService AddLog(LogEntityModel log)
    {
        var addedLog = _notification.LogEntities
            .FirstOrDefault(c => c.EntityName == log.EntityName && c.Pk == log.Pk && c.Action == log.Action);
        if (addedLog != null)
            _notification.LogEntities.Remove(addedLog);
        _notification.LogEntities.Add(log);

        return this;
    }

    public IActivityLogService AddLogs(IEnumerable<LogEntityModel> logs)
    {
        if (logs?.Any() != true)
            return this;
        foreach (var log in logs)
            AddLog(log);
        return this;
    }

    public IActivityLogService AddRelatedEntity(LogRelatedEntityModel entity)
    {
        var addedEntity = _notification.LogRelatedEntities
            .FirstOrDefault(c => c.EntityName == entity.EntityName && c.Pk == entity.Pk);
        if (addedEntity != null)
            _notification.LogRelatedEntities.Remove(addedEntity);
        _notification.LogRelatedEntities.Add(entity);

        return this;
    }

    public IActivityLogService AddRelatedEntities(IEnumerable<LogRelatedEntityModel> entities)
    {
        if (entities?.Any() != true)
            return this;
        foreach (var entity in entities)
            AddRelatedEntity(entity);
        return this;
    }

    public IActivityLogService EnableNotifications(IEnumerable<long> notifiedUserIds)
    {
        if (notifiedUserIds?.Any() == true)
        {
            _notification.NotifiedUserIds ??= [];
            _notification.NotifiedUserIds = [.. _notification.NotifiedUserIds.Union(notifiedUserIds).Distinct()];
        }
        return this;
    }

    public int Commit()
    {
        return CommitAsync().Result;
    }

    public async Task<int> CommitAsync()
    {
        if (_isCommited)
            return 0;

        if (_notification.LogEntities.Any(c => c.OldValue != c.NewValue))
        {
            // Publish
            await _mediator.Publish(_notification.SetCurrentUser(_notification.CurrentUser, _notification.CurrentUserId));
            _isCommited = true;
        }
        return _notification.LogEntities.Count;
    }
}