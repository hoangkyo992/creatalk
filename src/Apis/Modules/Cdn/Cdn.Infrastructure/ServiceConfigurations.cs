using Amazon;
using Amazon.S3;
using Cdn.Application.Common;
using Cdn.Application.Shared.Configurations;
using Cdn.Domain.Services;
using Cdn.Domain.Shared;
using Cdn.Infrastructure.Persistence;
using Cdn.Infrastructure.Services;
using Common.Application.Services;
using Common.Infrastructure.Services;
using Common.Infrastructure.Services.ActivityLog;
using HungHd.Shared;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Cdn.Infrastructure;

public static partial class ServiceConfigurations
{
    public static IServiceCollection AddCdnServices(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddHttpClient();
        services.AddScoped<IActivityLogService, ActivityLogService>();
        services.AddTransient<IFeatureProvider, FeatureProvider>();
        AddCdnConfiguration(services, configuration);
        AddAwsServices(services, configuration);
        AddCdnDbContext(services, configuration);
        return services;
    }

    public static IServiceCollection AddIdentityService(this IServiceCollection services)
    {
        services.AddScoped<ISettingService, HttpSettingService>();
        services.AddScoped<IIdentityService, HttpIdentityService>();
        services.AddScoped<ICredentialService, HttpCredentialService>();

        return services;
    }

    public static IServiceCollection AddCdnService(this IServiceCollection services, bool useInProcessService = true)
    {
        if (useInProcessService)
        {
            services.AddScoped<ICdnService, CdnService>();
        }
        else
        {
            services.AddScoped<ICdnService, HttpCdnService>();
        }
        return services;
    }

    internal static IServiceCollection AddCdnConfiguration(this IServiceCollection services, IConfiguration configuration)
    {
        var cdnServerOptions = new CdnServerConfiguration();
        IConfigurationSection cdnServerConfigSection = configuration.GetSection(CdnServerConfiguration.ConfigSection);
        cdnServerConfigSection.Bind(cdnServerOptions);
        services.Configure<CdnServerConfiguration>(cdnServerConfigSection);

        return services;
    }

    internal static IServiceCollection AddAwsServices(this IServiceCollection services, IConfiguration configuration)
    {
        var s3Options = new AwsS3Options();
        IConfigurationSection s3Config = configuration.GetSection(AwsS3Options.Section);
        s3Config.Bind(s3Options);
        services.Configure<AwsS3Options>(s3Config);
        services.AddScoped<IAmazonS3>(factory => new AmazonS3Client(s3Options.AccessKeyId, s3Options.SecretKey, RegionEndpoint.GetBySystemName(s3Options.Region)));
        if (s3Options.IsValid())
        {
            services.AddScoped<IStorageService, S3StorageService>();
        }
        else
        {
            services.AddScoped<IStorageService, DbStorageService>();
        }
        return services;
    }

    internal static void AddCdnDbContext(IServiceCollection services, IConfiguration configuration)
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
                    npgsqlOptions.MigrationsHistoryTable(string.Format(DbConstants.MigrationTable, "Cdn"), DbConstants.SchemaName);
                });
                options.EnableSensitiveDataLogging(configuration.GetValue("DbContext:AppDbContext:EnableSensitiveDataLogging", false));
            }
        });
        services.AddScoped<IAppContext>(provider => provider.GetRequiredService<AppDbContext>());
    }
}