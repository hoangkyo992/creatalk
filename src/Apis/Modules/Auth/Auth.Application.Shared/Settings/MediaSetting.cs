namespace Auth.Application.Shared.Settings;

public class MediaSetting : ISettingObject
{
    public ImageSetting ImageSetting { get; set; } = new();
}

public class ImageSetting
{
    /// <summary>
    /// Crop thumbnail to exact dimensions (normally thumbnails are proportional)
    /// </summary>
    public bool CropExact { get; set; } = false;

    public ImageSize MediumImageSize { get; set; } = new(300, 300);
    public ImageSize LargeImageSize { get; set; } = new(1024, 1024);
    public ImageSize ThumbnailImageSize { get; set; } = new(150, 150);
}

public class ImageSize(int maxWidth = 1024, int maxHeight = 1024)
{
    public int MaxWidth { get; set; } = maxWidth;
    public int MaxHeight { get; set; } = maxHeight;
}