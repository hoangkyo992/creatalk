using PDFtoImage;

namespace Cdn.Infrastructure.Services;

internal class ImageResizer : IImageResizer
{
    public byte[] Resize(byte[] content, int width, int height)
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

    public byte[] CropImage(byte[] content, int width, int height, int xOffset, int yOffset)
    {
        try
        {
            using var image = new MagickImage(content);
            var geometry = new MagickGeometry(xOffset, yOffset, (uint)width, (uint)height);
            image.Crop(geometry);
            image.ResetPage();
            return image.ToByteArray();
        }
        catch
        {
            return content;
        }
    }

    [System.Diagnostics.CodeAnalysis.SuppressMessage("Interoperability", "CA1416:Validate platform compatibility", Justification = "<Pending>")]
    public byte[] ConvertToImage(byte[] content)
    {
        using var stream = new MemoryStream();
        using var bitmap = Conversion.ToImage(content, 0);
        bitmap.Encode(stream, SkiaSharp.SKEncodedImageFormat.Png, 100);
        return stream.ToArray();
    }
}