using System.ComponentModel;
using System.Reflection;
using Common.Domain.Common;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking.Internal;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyModel;

namespace Common.Infrastructure.Persistence;

public abstract partial class BaseDbContext
{
    protected readonly ICurrentUser? _currentUser;
    protected readonly IServiceProvider _serviceProvider;

    protected string CurrentUsername
    {
        get
        {
            return (_currentUser?.Id > 0 ? _currentUser : _serviceProvider.GetRequiredService<ICurrentUser>()).Username ?? "N/A";
        }
    }

    protected BaseDbContext(DbContextOptions options,
        IServiceProvider serviceProvider,
        ICurrentUser? currentUser)
        : base(options)
    {
        _currentUser = currentUser;
        _serviceProvider = serviceProvider;
        AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);
    }

    public override int SaveChanges()
    {
        return this.SaveChanges(true);
    }

    public override Task<int> SaveChangesAsync(bool acceptAllChangesOnSuccess, CancellationToken cancellationToken = default)
    {
        SetStringEmptyInsteadOfNull();
        return base.SaveChangesAsync(acceptAllChangesOnSuccess, cancellationToken);
    }

    public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        return this.SaveChangesAsync(true, cancellationToken);
    }

    private void SetStringEmptyInsteadOfNull()
    {
        string typeofString = typeof(string).Name;
        foreach (var entry in this.ChangeTracker.Entries<BaseEntity>())
        {
            if (entry.State == EntityState.Added)
            {
                entry.Entity.CreatedBy = entry.Entity.CreatedBy ?? CurrentUsername;
                entry.Entity.CreatedTime = DateTime.UtcNow;
            }
            else if (entry.State == EntityState.Modified)
            {
                entry.Entity.RowVersion += 1;
                entry.Entity.UpdatedBy = CurrentUsername;
                entry.Entity.UpdatedTime = DateTime.UtcNow;
            }

            if (entry.State == EntityState.Added || entry.State == EntityState.Modified)
            {
                var properties = entry.Entity
                    .GetType().GetProperties()
                    .Where(p => p.PropertyType.Name == typeofString)
                    .Where(p => p.GetCustomAttributes<DefaultValueAttribute>() != null);

                foreach (var prop in properties)
                {
                    var value = (string?)prop.GetValue(entry.Entity);
                    if (value == null)
                    {
                        var defaultValue = (string?)prop.GetCustomAttribute<DefaultValueAttribute>()?.Value;
                        prop.SetValue(entry.Entity, defaultValue, null);
                    }
                }
            }
        }
    }

    protected static IList<Type> GetEntityTypes(Type baseType)
    {
        return (from a in GetReferencingAssemblies()
                from t in a.DefinedTypes
                where t.IsSubclassOf(baseType)
                select t.AsType()).ToList();
    }

    protected static Dictionary<string, Type> GetDbSetProperties(DbContext context)
    {
        var dbSetProperties = new Dictionary<string, Type>();
        var properties = context.GetType().GetProperties();
        foreach (var property in properties)
        {
            var isDbSet = property.PropertyType.IsGenericType
                && (typeof(DbSet<>).IsAssignableFrom(property.PropertyType.GetGenericTypeDefinition()));
            if (isDbSet)
            {
                dbSetProperties.Add(property.Name, property.PropertyType.GetGenericArguments()[0]);
            }
        }
        return dbSetProperties;
    }

    protected static IEnumerable<Assembly> GetReferencingAssemblies()
    {
        var assemblies = new List<Assembly>();
        if (DependencyContext.Default != null)
        {
            foreach (var library in DependencyContext.Default.RuntimeLibraries)
            {
                try
                {
                    if (library.Type == "project" || library.Name.StartsWith("HungHd.") || library.Name.StartsWith("Vikiworld."))
                        assemblies.Add(Assembly.Load(new AssemblyName(library.Name)));
                }
                catch (Exception)
                {
                    // Ignore
                }
            }
        }
        return assemblies;
    }

    /// <summary>
    /// Applying BaseEntity rules to all entities that inherit from it.
    /// Define MethodInfo member that is used when model is built.
    /// </summary>
    protected static readonly MethodInfo SetGlobalQueryMethod
        = typeof(BaseDbContext).GetMethods(BindingFlags.Public | BindingFlags.Instance)
        .Single(t => t.IsGenericMethod && t.Name == "SetGlobalQuery");

    /// <summary>
    /// This method is called for every loaded entity type in OnModelCreating method.
    /// Here type is known through generic parameter and we can use EF Core methods.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="builder"></param>
    public virtual void SetGlobalQuery<T>(ModelBuilder builder) where T : BaseEntity
    {
        builder.Entity<T>().HasQueryFilter(p => !p.IsDeleted && p.Id > int.MaxValue);
        builder.Entity<T>().UseTpcMappingStrategy();
    }
}