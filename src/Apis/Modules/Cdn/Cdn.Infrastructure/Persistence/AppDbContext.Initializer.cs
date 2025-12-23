using Cdn.Domain.Entities;
using Cdn.Domain.Shared.Enums;
using HungHd.Shared;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Npgsql;
using Polly;
using Polly.Retry;

namespace Cdn.Infrastructure.Persistence;

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
            if (!await context.FileExtensions.AnyAsync())
            {
                context.FileExtensions.Add(new FileExtension
                {
                    Id = IDGenerator.GenerateId(),
                    MineType = "image/gif",
                    Name = ".gif",
                    TypeId = FileType.Image,
                });
                context.FileExtensions.Add(new FileExtension
                {
                    Id = IDGenerator.GenerateId(),
                    MineType = "image/heic",
                    Name = ".heic",
                    TypeId = FileType.Image,
                });
                context.FileExtensions.Add(new FileExtension
                {
                    Id = IDGenerator.GenerateId(),
                    MineType = "image/heic",
                    Name = ".heif",
                    TypeId = FileType.Image,
                });
                context.FileExtensions.Add(new FileExtension
                {
                    Id = IDGenerator.GenerateId(),
                    MineType = "image/jpeg",
                    Name = ".jpe",
                    TypeId = FileType.Image,
                });
                context.FileExtensions.Add(new FileExtension
                {
                    Id = IDGenerator.GenerateId(),
                    MineType = "image/jpeg",
                    Name = ".jpeg",
                    TypeId = FileType.Image,
                });
                context.FileExtensions.Add(new FileExtension
                {
                    Id = IDGenerator.GenerateId(),
                    MineType = "image/jpeg",
                    Name = ".jpg",
                    TypeId = FileType.Image,
                });
                context.FileExtensions.Add(new FileExtension
                {
                    Id = IDGenerator.GenerateId(),
                    MineType = "image/jpeg",
                    Name = ".pjpg",
                    TypeId = FileType.Image,
                });
                context.FileExtensions.Add(new FileExtension
                {
                    Id = IDGenerator.GenerateId(),
                    MineType = "image/jpeg",
                    Name = ".jfif",
                    TypeId = FileType.Image,
                });
                context.FileExtensions.Add(new FileExtension
                {
                    Id = IDGenerator.GenerateId(),
                    MineType = "image/png",
                    Name = ".png",
                    TypeId = FileType.Image,
                });
                context.FileExtensions.Add(new FileExtension
                {
                    Id = IDGenerator.GenerateId(),
                    MineType = "image/svg+xml",
                    Name = ".svgz",
                    TypeId = FileType.Image,
                });
                context.FileExtensions.Add(new FileExtension
                {
                    Id = IDGenerator.GenerateId(),
                    MineType = "image/svg+xml",
                    Name = ".svg",
                    TypeId = FileType.Image,
                });
                context.FileExtensions.Add(new FileExtension
                {
                    Id = IDGenerator.GenerateId(),
                    MineType = "image/webp",
                    Name = ".webp",
                    TypeId = FileType.Image,
                });
                context.FileExtensions.Add(new FileExtension
                {
                    Id = IDGenerator.GenerateId(),
                    MineType = "image/x-icon",
                    Name = ".ico",
                    TypeId = FileType.Image,
                });
                await context.SaveChangesAsync();
            }
            if (!await context.FileExtensions.AnyAsync(c => c.Name == ".pdf"))
            {
                context.FileExtensions.Add(new FileExtension
                {
                    Id = IDGenerator.GenerateId(),
                    MineType = "application/pdf",
                    Name = ".pdf",
                    TypeId = FileType.Document,
                });
                await context.SaveChangesAsync();
            }
        });
    }
}