using Auth.Domain.Shared.Constants;

namespace Auth.Domain.Entities;

[Table("Role", Schema = DbConstants.SchemaName)]
public class Role : BaseEntity
{
    public Role()
    {
        Users = [];
        Features = [];
    }

    [Required]
    [MaxLength(RoleConstants.ColumnsMaxLength.Name)]
    public string Name { get; set; }

    [Required]
    [MaxLength(RoleConstants.ColumnsMaxLength.Description)]
    public string Description { get; set; }

    public virtual ICollection<User> Users { get; set; }
    public virtual ICollection<RoleFeature> Features { get; set; }
}