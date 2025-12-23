using Common.Domain.Common;
using Microsoft.EntityFrameworkCore;

namespace Common.Infrastructure.Persistence;

public abstract partial class BaseDbContext
{
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        DatetimeConverter.UseUTCDatetimeConversion(modelBuilder);

        var dbSets = GetDbSetProperties(this);
        foreach (var method in from type in GetEntityTypes(typeof(BaseEntity))
                               where dbSets.Any(c => c.Value.Equals(type))
                               let method = SetGlobalQueryMethod.MakeGenericMethod(type)
                               select method)
        {
            method.Invoke(this, [modelBuilder]);
        }
    }
}