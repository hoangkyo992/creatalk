namespace Common.Application.Services;

public interface IActivityLogService
{
    /// <summary>
    /// Setup activity information
    /// </summary>
    /// <param name="label">Label</param>
    /// <param name="description">Activity description</param>
    /// <param name="currentUser">Authority</param>
    /// <param name="relatedItem">Link to entity where logs are displayed</param>
    IActivityLogService Setup(Enum label, string description, ICurrentUser currentUser, LogRelatedEntityModel? relatedItem = null);

    /// <summary>
    /// Setup activity information
    /// </summary>
    /// <param name="label">Label</param>
    /// <param name="description">Activity description</param>
    /// <param name="currentUser">Authority</param>
    /// <param name="relatedItem">Link to entity where logs are displayed</param>
    IActivityLogService Setup(string label, string description, ICurrentUser currentUser, LogRelatedEntityModel? relatedItem = null);

    /// <summary>
    /// Add a changed entity
    /// </summary>
    /// <param name="log"></param>
    IActivityLogService AddLog(LogEntityModel log);

    /// <summary>
    /// Add changed entities
    /// </summary>
    /// <param name="logs"></param>
    IActivityLogService AddLogs(IEnumerable<LogEntityModel> logs);

    /// <summary>
    /// Add a related entity
    /// </summary>
    /// <param name="entity"></param>
    IActivityLogService AddRelatedEntity(LogRelatedEntityModel entity);

    /// <summary>
    /// Add related entities
    /// </summary>
    /// <param name="entities"></param>
    IActivityLogService AddRelatedEntities(IEnumerable<LogRelatedEntityModel> entities);

    /// <summary>
    /// Create new notification item for these users after log item created
    /// </summary>
    /// <param name="notifiedUserIds"></param>
    /// <returns></returns>
    IActivityLogService EnableNotifications(IEnumerable<long> notifiedUserIds);

    /// <summary>
    /// Commit logs
    /// </summary>
    /// <returns></returns>
    int Commit();

    /// <summary>
    /// Commit logs
    /// </summary>
    /// <returns></returns>
    Task<int> CommitAsync();
}