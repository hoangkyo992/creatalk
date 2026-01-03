using Auth.Domain.Entities;
using Auth.Domain.Interfaces;
using Common.Domain;
using Common.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Auth.Infrastructure.Persistence;

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

    public virtual DbSet<User> Users { get; set; }
    public virtual DbSet<Role> Roles { get; set; }
    public virtual DbSet<UserRole> UserRoles { get; set; }
    public virtual DbSet<RoleFeature> RoleFeatures { get; set; }
    public virtual DbSet<UserSession> UserSessions { get; set; }
    public virtual DbSet<LogActivity> LogActivities { get; set; }
    public virtual DbSet<LogEntity> LogEntities { get; set; }
    public virtual DbSet<LogRelatedEntity> LogRelatedEntities { get; set; }
    public virtual DbSet<Setting> Settings { get; set; }
    public virtual DbSet<Credential> Credentials { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfiguration(new SequencesConfiguration());
        base.OnModelCreating(modelBuilder);
        modelBuilder.Entity<LogEntity>().HasQueryFilter(p => p.TenantId == TenantId);
        modelBuilder.Entity<LogActivity>().HasQueryFilter(p => p.TenantId == TenantId);
        modelBuilder.Entity<LogRelatedEntity>().HasQueryFilter(p => p.TenantId == TenantId);
    }
}