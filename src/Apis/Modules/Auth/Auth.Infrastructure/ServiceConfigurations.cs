using Auth.Application.Common;
using Auth.Application.Configurations;
using Auth.Domain.Interfaces;
using Auth.Domain.Shared;
using Auth.Infrastructure.Persistence;
using Auth.Infrastructure.Services;
using Common.Application.Services;
using Common.Infrastructure.Services;
using Common.Infrastructure.Services.ActivityLog;
using HungHd.Shared;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Auth.Infrastructure;

public static partial class ServiceConfigurations
{
    public static IServiceCollection AddAuthServices(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddHttpClient();
        services.Configure<AuthIdentityConfiguration>(configuration.GetSection(AuthIdentityConfiguration.ConfigSection));
        services.AddTransient<ITokenService, JwtTokenService>();
        services.AddTransient<IFeatureProvider, FeatureProvider>();
        services.AddScoped<IActivityLogService, ActivityLogService>();
        AddAuthDbContext(services, configuration);
        return services;
    }

    public static IServiceCollection AddIdentityService(this IServiceCollection services, bool useInProcessService = true)
    {
        if (useInProcessService)
        {
            services.AddScoped<ISettingService, SettingService>();
            services.AddScoped<IIdentityService, IdentityService>();
        }
        else
        {
            services.AddScoped<ISettingService, HttpSettingService>();
            services.AddScoped<IIdentityService, HttpIdentityService>();
        }
        return services;
    }

    internal static void AddAuthDbContext(IServiceCollection services, IConfiguration configuration)
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
                    npgsqlOptions.MigrationsHistoryTable(string.Format(DbConstants.MigrationTable, "Auth"), DbConstants.SchemaName);
                });
                options.EnableSensitiveDataLogging(configuration.GetValue("DbContext:AppDbContext:EnableSensitiveDataLogging", false));
            }
        });
        services.AddScoped<IAppContext>(provider => provider.GetRequiredService<AppDbContext>());
    }
}