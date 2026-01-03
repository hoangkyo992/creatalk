using Auth.Domain.Shared.Constants;

namespace Auth.Domain.Entities;

[Table("Credential", Schema = DbConstants.SchemaName)]
public class Credential : BaseEntity
{
    public Credential()
    {
    }

    [Required]
    [MaxLength(CredentialConstants.ColumnsMaxLength.Key)]
    public string Key { get; set; }

    [Required]
    public string Value { get; set; }
}