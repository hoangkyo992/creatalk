namespace Cdn.Domain.Entities;

[Table("Folder", Schema = DbConstants.SchemaName)]
public class Folder : TenantBaseEntity
{
    public Folder()
    {
    }

    [Required]
    [MaxLength(FolderConstants.ColumnsMaxLength.Name)]
    public string Name { get; set; }

    public bool IsEmpty { get; set; }

    public FolderStatus StatusId { get; set; }

    public long? ParentId { get; set; }
    public virtual Folder Parent { get; set; }
}