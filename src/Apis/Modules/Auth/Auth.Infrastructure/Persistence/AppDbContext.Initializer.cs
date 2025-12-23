using System.Net.WebSockets;
using Auth.Application.Common;
using Auth.Application.Shared;
using Auth.Application.Shared.Settings;
using Auth.Domain.Entities;
using Auth.Domain.Shared.Enums;
using HungHd.Shared.Utilities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Npgsql;
using Polly;
using Polly.Retry;

namespace Auth.Infrastructure.Persistence;

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

    public async Task SeedAsync(AppDbContext context, ILogger<AppDbContextInitializer> logger)
    {
        ArgumentNullException.ThrowIfNull(context);

        var policy = CreatePolicy(logger, nameof(AppDbContextInitializer));
        await policy.ExecuteAsync(async () =>
        {
            if (!await context.Roles.IgnoreQueryFilters().AnyAsync())
            {
                context.Roles.Add(new Role
                {
                    Id = (int)SystemRole.SuperAdmin,
                    CreatedBy = "System",
                    CreatedTime = DateTime.UtcNow,
                    Name = "SystemAdmin",
                    Description = "System Administrator"
                });
                await context.SaveChangesAsync();
            }
            if (!await context.Users.IgnoreQueryFilters().AnyAsync())
            {
                context.Users.Add(new User
                {
                    Id = 1,
                    Username = "sysadmin",
                    Email = "admin@gmail.com",
                    CreatedBy = "System",
                    CreatedTime = DateTime.UtcNow,
                    DisplayName = "sysadmin",
                    Password = PasswordBuilder.Create("Creatalk!@2024"),
                    StatusId = UserStatus.Active
                });
                await context.SaveChangesAsync();
            }

            if (!await context.UserRoles.IgnoreQueryFilters().AnyAsync(c => c.UserId == 1))
            {
                context.UserRoles.Add(new UserRole
                {
                    UserId = 1,
                    RoleId = (int)SystemRole.SuperAdmin,
                    CreatedBy = "System",
                    CreatedTime = DateTime.UtcNow,
                });
                await context.SaveChangesAsync();
            }

            if (!await context.Settings.Where(c => c.Key == SettingKeys.Media).AnyAsync())
            {
                context.Settings.Add(new Setting
                {
                    Key = SettingKeys.Media,
                    CreatedBy = "System",
                    CreatedTime = DateTime.UtcNow,
                    Value = new MediaSetting().ToJson()
                });
                await context.SaveChangesAsync();
            }

            if (!await context.Settings.Where(c => c.Key == SettingKeys.General).AnyAsync())
            {
                context.Settings.Add(new Setting
                {
                    Key = SettingKeys.General,
                    CreatedBy = "System",
                    CreatedTime = DateTime.UtcNow,
                    Value = new GeneralSetting().ToJson()
                });
                await context.SaveChangesAsync();
            }
        });
    }
}