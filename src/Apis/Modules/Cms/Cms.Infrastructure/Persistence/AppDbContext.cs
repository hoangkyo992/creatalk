using Cms.Domain.Entities;
using Common.Domain;
using Common.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Cms.Infrastructure.Persistence;

public partial class AppDbContext(DbContextOptions<AppDbContext> options,
    IServiceProvider serviceProvider,
    ICurrentUser currentUser)
    : BaseDbContext(options, serviceProvider, currentUser), IAppContext
{
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        if (!optionsBuilder.IsConfigured)
        {
            // .
        }
    }

    public virtual DbSet<MessageProvider> MessageProviders { get; set; }
    public virtual DbSet<Attendee> Attendees { get; set; }
    public virtual DbSet<AttendeeMessage> AttendeeMessages { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfiguration(new SequencesConfiguration());
        base.OnModelCreating(modelBuilder);
    }
}