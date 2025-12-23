using Cdn.Application.Shared.Configurations;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Npgsql;
using Polly;
using Polly.Retry;

namespace Cms.Infrastructure.Persistence;

public class AppDbContextInitializer
{
    private static AsyncRetryPolicy CreatePolicy(ILogger<AppDbContextInitializer> logger, string prefix, int retries = 3)
    {
        return Policy.Handle<NpgsqlException>().
            WaitAndRetryAsync(
                retryCount: retries,
                sleepDurationProvider: retry => TimeSpan.FromSeconds(5),
                onRetry: (exception, timeSpan, retry, ctx) => logger.LogWarning(exception, "[{Prefix}] Exception {ExceptionType} with message {Message} detected on attempt {Retry} of {Eetries}", prefix, exception.GetType().Name, exception.Message, retry, retries));
    }

    public async Task SeedAsync(AppDbContext context, IServiceProvider serviceProvider, ILogger<AppDbContextInitializer> logger)
    {
        ArgumentNullException.ThrowIfNull(context);
        var configuration = serviceProvider.GetRequiredService<IConfiguration>();
        var cdnServerOptions = new CdnServerConfiguration();
        IConfigurationSection cdnServerConfigSection = configuration.GetSection(CdnServerConfiguration.ConfigSection);
        cdnServerConfigSection.Bind(cdnServerOptions);

        var policy = CreatePolicy(logger, nameof(AppDbContextInitializer));
        await policy.ExecuteAsync(async () =>
        {
            await Task.CompletedTask;
        });
    }
}