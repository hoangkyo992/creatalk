namespace Common.Application.Services;

public class FileItem
{
    [JsonConverter(typeof(ZCodeJsonConverter))]
    public long Id { get; set; }

    public string Name { get; init; }
    public string Url { get; init; }
    public string PublicUrl { get; init; }

    [JsonConverter(typeof(ZCodeJsonConverter))]
    public long FolderId { get; init; }

    public int FileTypeId { get; init; }
    public string FolderName { get; init; }
    public int Size { get; init; }
    public string Extension { get; init; }
    public string Properties { get; init; }
    public string? MineType { get; set; }
}