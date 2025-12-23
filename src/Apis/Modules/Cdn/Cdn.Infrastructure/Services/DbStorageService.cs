using Cdn.Domain.Services;

namespace Cdn.Infrastructure.Services;

public class DbStorageService : IStorageService
{
    public async Task<string> UploadAsync(Domain.Entities.File file, MemoryStream stream, CancellationToken cancellationToken = default)
    {
        file.Content = stream.ToArray();
        file.Size = file.Content.Length;
        return await Task.FromResult(string.Empty);
    }

    public async Task<byte[]> DownloadAsync(string url, CancellationToken cancellationToken = default)
    {
        return await Task.FromResult(Array.Empty<byte>());
    }

    public async Task<string> GetDownloadUrlAsync(string url, CancellationToken cancellationToken = default)
    {
        return await Task.FromResult(string.Empty);
    }
}