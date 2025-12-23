namespace Cdn.Application.Shared.Resources;

public partial struct ErrorMessages
{
    public const string FILE_NOT_FOUND = nameof(FILE_NOT_FOUND);
    public const string FILE_TOO_BIG = nameof(FILE_TOO_BIG);
    public const string FILE_NAME_EXISTED = nameof(FILE_NAME_EXISTED);
    public const string FILE_LOCKED = nameof(FILE_LOCKED);
    public const string FILE_EXTENSION_NOT_SUPPORTED = nameof(FILE_EXTENSION_NOT_SUPPORTED);
    public const string MAX_FILES_PER_FOLDER_EXCEEDED = nameof(MAX_FILES_PER_FOLDER_EXCEEDED);
    public const string NAME_CONTAINS_INVALID_CHARACTERS = nameof(NAME_CONTAINS_INVALID_CHARACTERS);

    public const string FOLDER_NOT_FOUND = nameof(FOLDER_NOT_FOUND);
    public const string FOLDER_NAME_EXISTED = nameof(FOLDER_NAME_EXISTED);
    public const string FOLDER_LOCKED = nameof(FOLDER_LOCKED);
    public const string FOLDER_PARENT_INVALID = nameof(FOLDER_PARENT_INVALID);

    public const string ALBUM_NOT_FOUND = nameof(ALBUM_NOT_FOUND);
    public const string ALBUM_NAME_EXISTED = nameof(ALBUM_NAME_EXISTED);
}