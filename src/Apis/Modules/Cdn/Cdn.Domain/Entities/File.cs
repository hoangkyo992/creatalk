using System.ComponentModel;

namespace Cdn.Domain.Entities;

[Table("File", Schema = DbConstants.SchemaName)]
public class File : BaseEntity
{
    public File()
    {
    }

    [Required]
    [MaxLength(FileConstants.ColumnsMaxLength.Name)]
    public string Name { get; set; }

    [Required]
    [MaxLength(FileConstants.ColumnsMaxLength.Url)]
    public string Url { get; set; }

    [Required]
    [MaxLength(FileConstants.ColumnsMaxLength.Extension)]
    public string Extension { get; set; }

    public byte[] Content { get; set; }

    [MaxLength(FileConstants.ColumnsMaxLength.StorageUrl)]
    public string? StorageUrl { get; set; }

    public int Size { get; set; }

    public FileType TypeId { get; set; }

    [Required, DefaultValue("")]
    public string Properties { get; set; }

    public FileStatus StatusId { get; set; }

    public long FolderId { get; set; }

    public virtual Folder Folder { get; set; }
}