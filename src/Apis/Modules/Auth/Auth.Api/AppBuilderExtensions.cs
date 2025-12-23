using Auth.Infrastructure.Persistence;
using Common.Infrastructure.Migrations;

namespace Auth.Api;

public static class AppBuilderExtensions
{
    public static WebApplication MigrateAuthDbContexts(this WebApplication app)
    {
        try
        {
            app.Logger.LogInformation("Seeding Auth database...");
            app.MigrateDbContext<AppDbContext>((context, services) =>
            {
                var logger = services.GetRequiredService<ILogger<AppDbContextInitializer>>();
                new AppDbContextInitializer().SeedAsync(context, logger).Wait();
            });
        }
        catch (Exception ex)
        {
            app.Logger.Log(LogLevel.Error, ex, "Seeding Auth database...failed");
        }
        return app;
    }

    public static WebApplication UseAuthServices(this WebApplication app)
    {
        app.MigrateAuthDbContexts();
        return app;
    }
}