namespace Auth.Domain.Entities;

[Table("UserRole", Schema = DbConstants.SchemaName)]
public class UserRole : BaseEntity
{
    public long UserId { get; set; }
    public long RoleId { get; set; }
    public DateTime? RevokedAt { get; set; }
    public virtual User User { get; set; }
    public virtual Role Role { get; set; }
}