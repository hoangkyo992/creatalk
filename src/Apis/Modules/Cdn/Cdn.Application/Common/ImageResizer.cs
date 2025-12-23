using ImageMagick;

namespace Cdn.Application.Common;

public static class ImageResizer
{
    public static byte[] Resize(byte[] content, int width, int height)
    {
        try
        {
            using var image = new MagickImage(content);
            var widthRate = (double)image.Width / width;
            var heightRate = (double)image.Height / height;
            var scaleRate = Math.Max(widthRate, heightRate);
            var newWidth = (uint)(image.Width / scaleRate);
            var newHeight = (uint)(image.Height / scaleRate);
            image.Resize(newWidth, newHeight);
            return image.ToByteArray();
        }
        catch
        {
            return content;
        }
    }
}