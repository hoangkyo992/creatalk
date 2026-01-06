namespace Cdn.Infrastructure.Services;

public class FilePropertyBuilder : IFilePropertyBuilder
{
    public string GetProperties(byte[] content, FileType typeId)
    {
        var properties = new List<string>();
        if (typeId == FileType.Image)
        {
            try
            {
                using var image = new MagickImage(content);
                properties.Add($"Width={image.Width}");
                properties.Add($"Height={image.Height}");
            }
            catch
            {
                //.
            }
        }
        return string.Join(";", properties);
    }
}