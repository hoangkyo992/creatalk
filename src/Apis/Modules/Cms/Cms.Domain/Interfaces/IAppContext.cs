using Cms.Domain.Entities;

namespace Cms.Domain.Interfaces;

public interface IAppContext : IDbContext, ISeqDbContext
{
    DbSet<Attendee> Attendees { get; }
    DbSet<AttendeeMessage> AttendeeMessages { get; }
    DbSet<MessageProvider> MessageProviders { get; }
}