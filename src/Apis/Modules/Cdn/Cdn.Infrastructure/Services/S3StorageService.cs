using Amazon.S3;
using Amazon.S3.Transfer;
using Cdn.Application.Shared.Configurations;
using Cdn.Domain.Services;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Cdn.Infrastructure.Services;

public class S3StorageService(IAmazonS3 amazonS3,
    ILogger<S3StorageService> logger,
    IOptions<AwsS3Options> awsOptions) : IStorageService
{
    private readonly IAmazonS3 _amazonS3 = amazonS3;
    private readonly AwsS3Options _awsS3Options = awsOptions.Value;
    private readonly ILogger<S3StorageService> _logger = logger;

    public async Task<string> UploadAsync(Domain.Entities.File file, MemoryStream stream, CancellationToken cancellationToken = default)
    {
        var fileTransferUtility = new TransferUtility(_amazonS3);
        var key = $"{DateTime.UtcNow:yyyyMMdd}/{file.Name}";
        int size = (int)stream.Length;
        TransferUtilityUploadRequest fileTransferUtilityRequest = new()
        {
            Key = key,
            BucketName = _awsS3Options.PublicBucketName,
            InputStream = stream,
            AutoCloseStream = true,
            CannedACL = S3CannedACL.PublicRead,
            StorageClass = S3StorageClass.ReducedRedundancy
        };
        await fileTransferUtility.UploadAsync(fileTransferUtilityRequest, cancellationToken);

        var url = $"{_awsS3Options.PublicStorageServerUrl}/{key}";
        file.Url = url;
        file.Size = size;
        file.Content = [];
        return url;
    }

    public async Task<byte[]> DownloadAsync(string url, CancellationToken cancellationToken = default)
    {
        try
        {
            var key = url.Replace(_awsS3Options.PublicStorageServerUrl, "")[1..];
            using var stream = await _amazonS3.GetObjectStreamAsync(_awsS3Options.PublicBucketName, key, default, cancellationToken);
            using var output = new MemoryStream();
            stream.CopyTo(output);
            return output.ToArray();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, nameof(S3StorageService));
            return [];
        }
    }

    public async Task<string> GetDownloadUrlAsync(string url, CancellationToken cancellationToken = default)
    {
        try
        {
            var key = url.Replace(_awsS3Options.PublicStorageServerUrl, "")[1..];
            var preSignedUrl = _amazonS3.GetPreSignedURL(new Amazon.S3.Model.GetPreSignedUrlRequest
            {
                BucketName = _awsS3Options.PublicBucketName,
                Key = key,
                Expires = DateTime.UtcNow.AddDays(_awsS3Options.PreSignedUrlExpireDays)
            });
            return await Task.FromResult(preSignedUrl);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, nameof(S3StorageService));
            return string.Empty;
        }
    }
}