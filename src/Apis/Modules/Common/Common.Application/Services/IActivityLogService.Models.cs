using Common.Domain.Enums;

namespace Common.Application.Services;

public abstract class LogEntityBase
{
    public string Pk { get; protected set; } = string.Empty;
    public string EntityName { get; protected set; } = string.Empty;
}

public class LogEntityModel : LogEntityBase
{
    public LogEntityModel() => ActionTime = DateTime.UtcNow;

    public LogEntityModel(string entity, string pk)
        : this()
    {
        EntityName = entity;
        Pk = pk;
    }

    public LogEntityModel(string entity, long pk)
        : this(entity, pk.ToString())
    {
    }

    public LogEntityModel(string entity, Guid pk)
        : this(entity, pk.ToString())
    {
    }

    public DateTime ActionTime { get; private set; }
    public ActivityLogAction Action { get; set; }
    public string Description { get; set; } = string.Empty;
    public object OldValue { get; set; } = new object();
    public object NewValue { get; set; } = new object();
}

public class LogRelatedEntityModel : LogEntityBase
{
    public LogRelatedEntityModel()
    {
    }

    public LogRelatedEntityModel(string entity, string pk)
        : this()
    {
        EntityName = entity;
        Pk = pk.ToString();
    }

    public LogRelatedEntityModel(string entity, long pk)
        : this(entity, pk.ToString())
    {
    }

    public LogRelatedEntityModel(string entity, Guid pk)
        : this(entity, pk.ToString())
    {
    }

    public string Description { get; set; } = string.Empty;
}

public class ActivityLogEventItem : ApplicationEvent
{
    public string RequestId { get; set; } = Guid.NewGuid().ToString();
    public DateTime CreatedDate { get; set; } = DateTime.UtcNow;
    public string Label { get; set; }
    public string Description { get; set; } = string.Empty;
    public string Action { get; set; } = string.Empty;
    public string MethodName { get; set; } = string.Empty;
    public string Source { get; set; } = string.Empty;
    public string IpAddress { get; set; } = string.Empty;
    public IList<LogEntityModel> LogEntities { get; set; } = [];
    public IList<LogRelatedEntityModel> LogRelatedEntities { get; set; } = [];
    public IList<long> NotifiedUserIds { get; set; } = [];
}