using HungHd.Shared;

namespace Common.Domain.Common;

public abstract class BaseEntity
{
    protected BaseEntity()
    {
        Id = IDGenerator.GenerateId();
    }

    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.None)]
    public long Id { get; set; }

    public bool IsDeleted { get; set; }

    public DateTime CreatedTime { get; set; }

    [Required]
    [MaxLength(200)]
    public string CreatedBy { get; set; }

    public DateTime? UpdatedTime { get; set; }

    [MaxLength(200)]
    public string? UpdatedBy { get; set; }

    [ConcurrencyCheck]
    public int RowVersion { get; set; }
}