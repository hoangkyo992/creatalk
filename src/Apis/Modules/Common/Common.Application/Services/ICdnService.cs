namespace Common.Application.Services;

public interface ICdnService
{
    Task<FileItem?> GetFileAsync(long id, CancellationToken cancellationToken = default);
}