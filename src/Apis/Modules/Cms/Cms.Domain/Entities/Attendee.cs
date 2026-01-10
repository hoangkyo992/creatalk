using Cms.Domain.Shared.Enums;

namespace Cms.Domain.Entities;

[Table("Attendee", Schema = DbConstants.SchemaName)]
public class Attendee : TenantBaseEntity
{
    [Required]
    [MaxLength(AttendeeConstants.ColumnsMaxLength.FirstName)]
    public string FirstName { get; set; }

    [Required]
    [MaxLength(AttendeeConstants.ColumnsMaxLength.LastName)]
    public string LastName { get; set; }

    [Required]
    [MaxLength(AttendeeConstants.ColumnsMaxLength.FullName)]
    public string FullName { get; set; }

    [Required]
    [DefaultValue("")]
    [MaxLength(AttendeeConstants.ColumnsMaxLength.Email)]
    public string Email { get; set; }

    [Required]
    [MaxLength(AttendeeConstants.ColumnsMaxLength.PhoneNumber)]
    public string PhoneNumber { get; set; }

    [Required]
    [DefaultValue("")]
    [MaxLength(AttendeeConstants.ColumnsMaxLength.TicketNumber)]
    public string TicketNumber { get; set; }

    [Required]
    [DefaultValue("")]
    [MaxLength(AttendeeConstants.ColumnsMaxLength.TicketPath)]
    public string TicketPath { get; set; }

    [Required]
    [DefaultValue("")]
    [MaxLength(AttendeeConstants.ColumnsMaxLength.TicketZone)]
    public string TicketZone { get; set; }

    public long TicketId { get; set; }

    public AttendeeStatus StatusId { get; set; }

    public virtual ICollection<AttendeeMessage> Messages { get; set; } = [];
}