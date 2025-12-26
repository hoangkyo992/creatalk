using Cms.Domain.Shared.Enums;

namespace Cms.Domain.Entities;

[Table("AttendeeMessage", Schema = DbConstants.SchemaName)]
public class AttendeeMessage : TenantBaseEntity
{
    public MessageStatus StatusId { get; set; }
    public string RequestPayload { get; set; }
    public string ResponsePayload { get; set; }
    public DateTime? SentAt { get; set; }

    [DefaultValue("")]
    [MaxLength(AttendeeMessageConstants.ColumnsMaxLength.MessageId)]
    public string MessageId { get; set; }

    public DateTime? UserReceivedAt { get; set; }
    public string? EventPayload { get; set; }

    public long AttendeeId { get; set; }
    public virtual Attendee Attendee { get; set; }
    public long ProviderId { get; set; }
    public virtual MessageProvider Provider { get; set; }
}