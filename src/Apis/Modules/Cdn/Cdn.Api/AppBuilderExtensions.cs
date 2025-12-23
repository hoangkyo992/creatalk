using Cdn.Infrastructure.Persistence;
using Common.Infrastructure.Migrations;

namespace Cdn.Api;

public static class AppBuilderExtensions
{
    public static WebApplication MigrateCdnDbContexts(this WebApplication app)
    {
        try
        {
            app.Logger.LogInformation("Seeding Cdn database...");
            app.MigrateDbContext<AppDbContext>((context, services) =>
            {
                var logger = services.GetRequiredService<ILogger<AppDbContextInitializer>>();
                new AppDbContextInitializer().SeedAsync(context, logger).Wait();
            });
        }
        catch (Exception ex)
        {
            app.Logger.Log(LogLevel.Error, ex, "Seeding Cdn database...failed");
        }
        return app;
    }

    public static WebApplication UseCdnServices(this WebApplication app)
    {
        app.MigrateCdnDbContexts();
        return app;
    }
}