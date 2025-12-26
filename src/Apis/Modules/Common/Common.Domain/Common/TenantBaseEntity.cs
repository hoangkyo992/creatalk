namespace Common.Domain.Common;

public abstract class TenantBaseEntity : BaseEntity
{
    public long TenantId { get; set; }
}