using HungHd.Shared;

namespace Auth.Domain.Entities;

[Table("LogActivity", Schema = DbConstants.SchemaName)]
public class LogActivity
{
    public LogActivity()
    {
        Id = IDGenerator.GenerateId();
    }

    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.None)]
    public long Id { get; set; }

    public long TenantId { get; set; }

    public long UserId { get; set; }

    [MaxLength(255)]
    public string Label { get; set; }

    [Required]
    [MaxLength(255)]
    public string Username { get; set; }

    public DateTime Time { get; set; }

    [Required]
    [MaxLength(100)]
    public string IpAddress { get; set; }

    [Required]
    [MaxLength(255)]
    public string Source { get; set; }

    [Required]
    [MaxLength(255)]
    public string MethodName { get; set; }

    [Required]
    [MaxLength(255)]
    public string Action { get; set; }

    [Required]
    [MaxLength(1024)]
    public string Description { get; set; }

    [MaxLength(255)]
    public string RequestId { get; set; }
}