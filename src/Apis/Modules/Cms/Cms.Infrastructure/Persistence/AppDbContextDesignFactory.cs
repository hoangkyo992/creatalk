using Cms.Domain.Shared;
using Common.Infrastructure;
using HungHd.Shared;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Cms.Infrastructure.Persistence;

public class AppDbContextDesignFactory : IDesignTimeDbContextFactory<AppDbContext>
{
    public AppDbContext CreateDbContext(string[] args)
    {
        IConfigurationRoot configuration = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json")
            .AddJsonFile("appsettings.Development.json")
            .Build();

        string conn = configuration.GetConnectionString("AppDbContext") ?? string.Empty;
        var optionsBuilder = new DbContextOptionsBuilder<AppDbContext>()
            .UseNpgsql(conn, npgsqlOptions =>
            {
                var version = configuration.GetValue("DbContext:AppDbContext:Version", DatabaseConstants.NpgsqlVersion) ?? DatabaseConstants.NpgsqlVersion;
                npgsqlOptions.SetPostgresVersion(new Version(version));
                npgsqlOptions.MigrationsAssembly(Assemblies.Infrastructure);
                npgsqlOptions.MigrationsHistoryTable(string.Format(DbConstants.MigrationTable, "Cms"), DbConstants.SchemaName);
            });

        return new AppDbContext(optionsBuilder.Options, new ServiceCollection().BuildServiceProvider(), new AnonymousUser());
    }
}