namespace Cdn.Domain.Services;

public interface IStorageService
{
    Task<string> UploadAsync(Entities.File file, MemoryStream stream, CancellationToken cancellationToken = default);

    Task<byte[]> DownloadAsync(string url, CancellationToken cancellationToken = default);

    Task<string> GetDownloadUrlAsync(string url, CancellationToken cancellationToken = default);
}