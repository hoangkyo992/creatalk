namespace Cms.Domain.Entities;

[Table("Attendee", Schema = DbConstants.SchemaName)]
public class Attendee : BaseEntity
{
    [Required]
    [MaxLength(AttendeeConstants.ColumnsMaxLength.FirstName)]
    public string FirstName { get; set; }

    [Required]
    [MaxLength(AttendeeConstants.ColumnsMaxLength.LastName)]
    public string LastName { get; set; }

    [Required]
    [MaxLength(AttendeeConstants.ColumnsMaxLength.Email)]
    public string Email { get; set; }

    [Required]
    [MaxLength(AttendeeConstants.ColumnsMaxLength.PhoneNumber)]
    public string PhoneNumber { get; set; }

    [Required]
    [DefaultValue("")]
    [MaxLength(AttendeeConstants.ColumnsMaxLength.TicketNumber)]
    public string TicketNumber { get; set; }

    public long TicketId { get; set; }

    public virtual ICollection<AttendeeMessage> Messages { get; set; } = [];
}