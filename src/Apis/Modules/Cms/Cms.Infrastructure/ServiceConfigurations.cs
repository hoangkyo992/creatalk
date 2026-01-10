using System.Net;
using Cms.Application.Common;
using Cms.Application.Services;
using Cms.Application.Services.Abstractions;
using Cms.Domain.Shared;
using Cms.Infrastructure.ApiClients.Zns;
using Cms.Infrastructure.Persistence;
using Cms.Infrastructure.Services;
using Common.Application.Services;
using Common.Infrastructure.Services;
using Common.Infrastructure.Services.ActivityLog;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Refit;

namespace Cms.Infrastructure;

public static partial class ServiceConfigurations
{
    public static IServiceCollection AddCmsServices(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddHttpClient();
        services.AddScoped<IActivityLogService, ActivityLogService>();
        services.AddTransient<IFeatureProvider, FeatureProvider>();
        services.AddSingleton<IQueueService, UnboundedChannelQueueService>();
        services.AddZnsServices(configuration);

        AddCmsDbContext(services, configuration);

        services.AddHostedService<SendMessageBackgroundService>();
        services.AddHostedService<EnqueueMessagesBackgroundService>();

        return services;
    }

    public static IServiceCollection AddIdentityService(this IServiceCollection services)
    {
        services.AddScoped<ISettingService, HttpSettingService>();
        services.AddScoped<IIdentityService, HttpIdentityService>();
        services.AddScoped<ICredentialService, HttpCredentialService>();

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

    internal static IAsyncPolicy<HttpResponseMessage> GetUnauthPolicy()
    {
        return Policy<HttpResponseMessage>
            .Handle<HttpRequestException>()
            .OrResult(msg => msg.StatusCode == HttpStatusCode.ServiceUnavailable)
            .OrResult(msg => msg.StatusCode == HttpStatusCode.BadGateway)
            .OrResult(msg => msg.StatusCode == HttpStatusCode.Unauthorized)
            .WaitAndRetryAsync(
                retryCount: 3,
                sleepDurationProvider: (retryAttempt) => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)),
                onRetryAsync: async (response, _, retryCount, context) =>
                {
                    if (response.Result?.StatusCode == HttpStatusCode.Unauthorized)
                    {
                        context.TryGetValue(nameof(IApiTokenResolver), out var resolver);
                        if (resolver is IApiTokenResolver tokenResolver)
                        {
                            await tokenResolver.ClearAccessToken();
                        }
                    }
                });
    }

    internal static IServiceCollection AddZnsServices(this IServiceCollection services, IConfiguration configuration)
    {
        var znsOptions = new ZnsOptions();
        IConfigurationSection znsConfig = configuration.GetSection(ZnsOptions.Section);
        znsConfig.Bind(znsOptions);
        services.Configure<ZnsOptions>(znsConfig);
        if (!znsOptions.IsValid())
        {
            throw new ArgumentException("Invalid ZNS configurations!!!");
        }

        services.AddTransient<ResponseLoggingHandler>();
        services.AddTransient<ZnsAuthorizationHandler>();
        services.TryAddKeyedSingleton<IApiTokenResolver, ZnsApiTokenResolver>(nameof(ZnsApiTokenResolver));
        services.AddRefitClient<IZnsApiClient>()
            .ConfigureHttpClient(c => c.BaseAddress = new Uri(znsOptions.ServiceUrl))
            .AddPolicyHandler(GetUnauthPolicy())
            .AddHttpMessageHandler<ResponseLoggingHandler>()
            .AddHttpMessageHandler<ZnsAuthorizationHandler>();
        services.AddKeyedSingleton<IMessageSender, ZnsMessageSender>("ZNS");

        return services;
    }
}