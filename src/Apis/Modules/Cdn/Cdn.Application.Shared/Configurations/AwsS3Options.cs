namespace Cdn.Application.Shared.Configurations;

public class AwsS3Options
{
    public const string Section = "AwsS3";
    public string AccessKeyId { get; set; } = string.Empty;
    public string SecretKey { get; set; } = string.Empty;
    public string PublicBucketName { get; set; } = string.Empty;
    public string PublicStorageServerUrl { get; set; } = string.Empty;
    public string Region { get; set; } = string.Empty;
    public int PreSignedUrlExpireDays { get; set; } = 1;

    public bool IsValid() => !string.IsNullOrWhiteSpace(AccessKeyId)
        && !string.IsNullOrWhiteSpace(SecretKey)
        && !string.IsNullOrWhiteSpace(PublicBucketName)
        && !string.IsNullOrWhiteSpace(PublicStorageServerUrl)
        && !string.IsNullOrWhiteSpace(Region);
}