namespace Cdn.Application.Services;

public interface IFilePropertyBuilder
{
    string GetProperties(byte[] content, FileType typeId);
}