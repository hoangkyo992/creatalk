namespace Cdn.Domain.Entities;

[Table("FileExtension", Schema = DbConstants.SchemaName)]
public class FileExtension : TenantBaseEntity
{
    public FileExtension()
    {
    }

    [Required]
    [MaxLength(FolderConstants.ColumnsMaxLength.Name)]
    public string Name { get; set; }

    [Required]
    public string MineType { get; set; }

    public FileType TypeId { get; set; }

    public bool IsDisabled { get; set; }
}