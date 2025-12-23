using Auth.Domain.Shared.Constants;
using Common.Domain.Interfaces;

namespace Auth.Domain.Entities;

[Table("Setting", Schema = DbConstants.SchemaName)]
public class Setting : BaseEntity, ISettingEntity
{
    public Setting()
    {
    }

    [Required]
    [MaxLength(SettingConstants.ColumnsMaxLength.Key)]
    public string Key { get; set; }

    [Required]
    public string Value { get; set; }
}