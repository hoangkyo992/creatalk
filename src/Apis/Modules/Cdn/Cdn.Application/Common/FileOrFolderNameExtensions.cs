namespace Cdn.Application.Common;

public static class FileOrFolderNameExtensions
{
    public static bool IsInvalidFileName(this string name)
    {
        return name.IndexOfAny(Path.GetInvalidFileNameChars()) > -1;
    }

    public static bool IsInvalidFolderName(this string name)
    {
        return name.IndexOfAny(Path.GetInvalidPathChars()) > -1;
    }
}