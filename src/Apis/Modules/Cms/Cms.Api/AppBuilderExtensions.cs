using Cms.Infrastructure.Persistence;
using Common.Infrastructure.Migrations;

namespace Cms.Api;

public static class AppBuilderExtensions
{
    public static WebApplication MigrateCmsDbContexts(this WebApplication app)
    {
        try
        {
            app.Logger.LogInformation("Seeding Cms database...");
            app.MigrateDbContext<AppDbContext>((context, services) =>
            {
                var logger = services.GetRequiredService<ILogger<AppDbContextInitializer>>();
                new AppDbContextInitializer().SeedAsync(context, services, logger).Wait();
            });
        }
        catch (Exception ex)
        {
            app.Logger.Log(LogLevel.Error, ex, "Seeding Cms database...failed");
        }
        return app;
    }

    public static WebApplication UseCmsServices(this WebApplication app)
    {
        app.MigrateCmsDbContexts();
        return app;
    }
}