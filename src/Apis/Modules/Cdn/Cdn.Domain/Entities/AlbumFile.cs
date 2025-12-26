using System.ComponentModel;

namespace Cdn.Domain.Entities;

[Table("AlbumFile", Schema = DbConstants.SchemaName)]
public class AlbumFile : TenantBaseEntity
{
    public AlbumFile()
    {
    }

    public int Index { get; set; }
    public long AlbumId { get; set; }
    public long FileId { get; set; }

    [Required]
    [MaxLength(AlbumConstants.ColumnsMaxLength.FileTitle)]
    [DefaultValue("")]
    public string Title { get; set; }

    [Required]
    [MaxLength(AlbumConstants.ColumnsMaxLength.FileDescription)]
    [DefaultValue("")]
    public string Description { get; set; }


    public virtual Album Album { get; set; }
    public virtual File File { get; set; }
}