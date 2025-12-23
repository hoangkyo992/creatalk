using Auth.Domain.Entities;
using Common.Domain.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Auth.Domain.Interfaces;

public interface IAppContext : IDbContext, ISeqDbContext
{
    DbSet<Role> Roles { get; }
    DbSet<RoleFeature> RoleFeatures { get; }
    DbSet<User> Users { get; }
    DbSet<UserRole> UserRoles { get; }
    DbSet<UserSession> UserSessions { get; }
    DbSet<LogActivity> LogActivities { get; }
    DbSet<LogEntity> LogEntities { get; }
    DbSet<LogRelatedEntity> LogRelatedEntities { get; }
    DbSet<Setting> Settings { get; }
}