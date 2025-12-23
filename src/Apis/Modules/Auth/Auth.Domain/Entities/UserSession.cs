using Auth.Domain.Shared.Constants;

namespace Auth.Domain.Entities;

[Table("UserSession", Schema = DbConstants.SchemaName)]
public class UserSession : BaseEntity
{
    public long UserId { get; set; }

    [Required]
    [MaxLength(UserConstants.ColumnsMaxLength.Username)]
    public string Username { get; set; }

    [Required]
    public string RefreshToken { get; set; }

    public DateTime StartTime { get; set; }

    public DateTime? EndTime { get; set; }

    [MaxLength(UserConstants.ColumnsMaxLength.Username)]
    public string? EndBy { get; set; }

    [MaxLength(UserSessionConstants.ColumnsMaxLength.IpAddress)]
    public string? IpAddress { get; set; }

    [MaxLength(UserSessionConstants.ColumnsMaxLength.Platform)]
    public string? Platform { get; set; }

    [MaxLength(UserSessionConstants.ColumnsMaxLength.Navigator)]
    public string? Navigator { get; set; }
}