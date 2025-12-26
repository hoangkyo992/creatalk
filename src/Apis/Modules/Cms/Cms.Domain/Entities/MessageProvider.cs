namespace Cms.Domain.Entities;

[Table("MessageProvider", Schema = DbConstants.SchemaName)]
public class MessageProvider : TenantBaseEntity
{
    [Required]
    [MaxLength(MessageProviderConstants.ColumnsMaxLength.Code)]
    public string Code { get; set; }

    [Required]
    [MaxLength(MessageProviderConstants.ColumnsMaxLength.Name)]
    public string Name { get; set; }

    [Required]
    public string Settings { get; set; }

    public bool IsDisabled { get; set; }

    public virtual ICollection<AttendeeMessage> Messages { get; set; } = [];
}