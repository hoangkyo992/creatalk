using Auth.Domain.Shared.Constants;
using Auth.Domain.Shared.Enums;

namespace Auth.Domain.Entities;

[Table("User", Schema = DbConstants.SchemaName)]
public class User : BaseEntity
{
    public User()
    {
        Roles = new HashSet<UserRole>();
    }

    [Required]
    [MaxLength(UserConstants.ColumnsMaxLength.Username)]
    public string Username { get; set; }

    [Required]
    [MaxLength(UserConstants.ColumnsMaxLength.DisplayName)]
    public string DisplayName { get; set; }

    [Required]
    public string Password { get; set; }

    [Required]
    [MaxLength(UserConstants.ColumnsMaxLength.Email)]
    public string Email { get; set; }

    [MaxLength(UserConstants.ColumnsMaxLength.Phone)]
    public string? Phone { get; set; }

    public UserStatus StatusId { get; set; }

    public string? Avatar { get; set; }

    public bool PasswordChanged { get; set; }

    public virtual ICollection<UserRole> Roles { get; set; }
}