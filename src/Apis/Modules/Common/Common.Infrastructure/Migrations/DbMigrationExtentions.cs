using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Npgsql;
using Polly;

namespace Common.Infrastructure.Migrations;

public static partial class DbMigrationExtensions
{
    public static WebApplication MigrateDbContext<TContext>(this WebApplication application, Action<TContext, IServiceProvider> seeder) where TContext : DbContext
    {
        using var scope = application.Services.CreateScope();
        var services = scope.ServiceProvider;
        var logger = services.GetRequiredService<ILogger<TContext>>();
        var context = services.GetRequiredService<TContext>();

        try
        {
            logger.LogInformation("Migrating database associated with context {DbContextName}", typeof(TContext).Name);

            var retries = 10;
            var retry = Policy.Handle<NpgsqlException>()
                .WaitAndRetry(
                    retryCount: retries,
                    sleepDurationProvider: retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)),
                    onRetry: (exception, timeSpan, retry, ctx) =>
                        logger.LogWarning(exception, "[{Prefix}] Exception {ExceptionType} with message {Message} detected on attempt {Retry} of {Retries}",
                                            nameof(TContext), exception.GetType().Name, exception.Message, retry, retries));
            retry.Execute(() => InvokeSeeder(seeder, context, services));
            logger.LogInformation("Migrated database associated with context {DbContextName}", typeof(TContext).Name);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "An error occurred while migrating the database used on context {DbContextName}", typeof(TContext).Name);
        }
        return application;
    }

    private static void InvokeSeeder<TContext>(Action<TContext, IServiceProvider> seeder, TContext context, IServiceProvider services)
        where TContext : DbContext
    {
        if (!context.Database.IsInMemory())
            context.Database.Migrate();
        seeder(context, services);
    }
}