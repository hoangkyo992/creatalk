using System.ComponentModel;
using HungHd.Shared;

namespace Auth.Domain.Entities;

[Table("LogEntity", Schema = DbConstants.SchemaName)]
public class LogEntity
{
    public LogEntity()
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

    [Required]
    [DefaultValue("")]
    [MaxLength(1024)]
    public string Description { get; set; }

    public char CRUD { get; set; }

    public DateTime Time { get; set; }

    [Required]
    public string OldValue { get; set; }

    [Required]
    public string NewValue { get; set; }
}