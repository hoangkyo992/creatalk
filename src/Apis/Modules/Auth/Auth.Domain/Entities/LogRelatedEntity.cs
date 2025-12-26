using HungHd.Shared;

namespace Auth.Domain.Entities;

[Table("LogRelatedEntity", Schema = DbConstants.SchemaName)]
public class LogRelatedEntity
{
    public LogRelatedEntity()
    {
        Id = IDGenerator.GenerateId();
    }

    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.None)]
    public long Id { get; set; }

    public long TenantId { get; set; }

    public long ActivityId { get; set; }

    [Required]
    [MaxLength(255)]
    public string EntityName { get; set; }

    [Required]
    [MaxLength(255)]
    public string Pk { get; set; }

    [MaxLength(1024)]
    public string Description { get; set; }
}