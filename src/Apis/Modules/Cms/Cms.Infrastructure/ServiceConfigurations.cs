using Cms.Application.Common;
using Cms.Domain.Shared;
using Cms.Infrastructure.Persistence;
using Common.Application.Services;
using Common.Infrastructure.Services;
using Common.Infrastructure.Services.ActivityLog;
using HungHd.Shared;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Cms.Infrastructure;

public static partial class ServiceConfigurations
{
    public static IServiceCollection AddCmsServices(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddHttpClient();
        services.AddScoped<IActivityLogService, ActivityLogService>();
        services.AddTransient<IFeatureProvider, FeatureProvider>();
        AddCmsDbContext(services, configuration);
        return services;
    }

    public static IServiceCollection AddIdentityService(this IServiceCollection services)
    {
        services.AddScoped<ISettingService, HttpSettingService>();
        services.AddScoped<IIdentityService, HttpIdentityService>();

        return services;
    }

    public static IServiceCollection AddCdnService(this IServiceCollection services)
    {
        services.AddScoped<ICdnService, HttpCdnService>();

        return services;
    }

    internal static void AddCmsDbContext(IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContext<AppDbContext>((options) =>
        {
            var useOnlyInMemoryDatabase = configuration.GetValue("DbContext:UseOnlyInMemoryDatabase", false);
            if (useOnlyInMemoryDatabase)
            {
                options.UseInMemoryDatabase("AppDbContext");
            }
            else
            {
                string conn = configuration.GetConnectionString("AppDbContext") ?? string.Empty;
                options.UseNpgsql(conn, npgsqlOptions =>
                {
                    var version = configuration.GetValue("DbContext:AppDbContext:Version", DatabaseConstants.NpgsqlVersion) ?? DatabaseConstants.NpgsqlVersion;
                    npgsqlOptions.SetPostgresVersion(new Version(version));
                    npgsqlOptions.MigrationsAssembly(Assemblies.Infrastructure);
                    npgsqlOptions.MigrationsHistoryTable(string.Format(DbConstants.MigrationTable, "Cms"), DbConstants.SchemaName);
                });
                options.EnableSensitiveDataLogging(configuration.GetValue("DbContext:AppDbContext:EnableSensitiveDataLogging", false));
            }
        });
        services.AddScoped<IAppContext>(provider => provider.GetRequiredService<AppDbContext>());
    }
}