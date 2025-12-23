namespace Cdn.Domain.Entities;

[Table("Album", Schema = DbConstants.SchemaName)]
public class Album : BaseEntity
{
    public Album()
    {
        Files = new HashSet<AlbumFile>();
    }

    [Required]
    [MaxLength(AlbumConstants.ColumnsMaxLength.Name)]
    public string Name { get; set; }

    [Required]
    [MaxLength(AlbumConstants.ColumnsMaxLength.Description)]
    public string Description { get; set; }

    public bool IsEmpty { get; set; }

    public virtual ICollection<AlbumFile> Files { get; set; }
}