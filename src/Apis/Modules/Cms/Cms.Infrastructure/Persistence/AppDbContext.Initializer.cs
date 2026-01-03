using Cms.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Npgsql;
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

        var policy = CreatePolicy(logger, nameof(AppDbContextInitializer));
        await policy.ExecuteAsync(async () =>
        {
            if (!await context.MessageProviders.AnyAsync())
            {
                context.MessageProviders.Add(new MessageProvider
                {
                    Id = IDGenerator.GenerateId(),
                    CreatedBy = "System",
                    CreatedTime = DateTime.UtcNow,
                    Name = "Email",
                    Code = "Email",
                    Settings = "",
                    IsDisabled = true
                });
                context.MessageProviders.Add(new MessageProvider
                {
                    Id = IDGenerator.GenerateId(),
                    CreatedBy = "System",
                    CreatedTime = DateTime.UtcNow,
                    Name = "ZNS",
                    Code = "ZNS",
                    Settings = "",
                    IsDisabled = false
                });
                await context.SaveChangesAsync();
            }
        });
    }
}