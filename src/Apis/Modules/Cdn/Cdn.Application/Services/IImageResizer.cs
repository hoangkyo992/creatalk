namespace Cdn.Application.Services;

public interface IImageResizer
{
    byte[] Resize(byte[] content, int width, int height);

    byte[] CropImage(byte[] content, int width, int height, int xOffset, int yOffset);

    byte[] ConvertToImage(byte[] content);
}