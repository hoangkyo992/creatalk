namespace Cms.Domain.Entities;

[Table("AttendeeMessage", Schema = DbConstants.SchemaName)]
public class AttendeeMessage : BaseEntity
{
    public int StatusId { get; set; }
    public string RequestPayload { get; set; }
    public string ResponsePayload { get; set; }

    [MaxLength(AttendeeMessageConstants.ColumnsMaxLength.MessageId)]
    public string MessageId { get; set; }

    public long AttendeeId { get; set; }
    public virtual Attendee Attendee { get; set; }
    public long ProviderId { get; set; }
    public virtual MessageProvider Provider { get; set; }
}