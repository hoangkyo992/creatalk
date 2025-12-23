namespace Auth.Domain.Entities;

[Table("RoleFeature", Schema = DbConstants.SchemaName)]
public class RoleFeature : BaseEntity
{
    public RoleFeature()
    {
    }

    public long RoleId { get; set; }

    public virtual Role Role { get; set; }

    public string Feature { get; set; }

    public string Action { get; set; }

    public override string ToString()
    {
        return $"{Feature.ToLower()}__{Action.ToLower()}";
    }
}