using Cdn.Application.Services;
using Cdn.Domain.Services;
using Cdn.Domain.Shared.Constants;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;

namespace Cdn.Application.Features.Files;

public class Upload
{
    public record Command : IRequest<ApiResult<Result>>
    {
        [JsonIgnore]
        public long? FolderId { get; init; }

        public IEnumerable<IFormFile> Files { get; init; }

        public Command(IEnumerable<IFormFile> files, long? folderId)
        {
            Files = files;
            FolderId = folderId;
        }
    }

    public record Result
    {
        public IEnumerable<Item> UploadedFiles { get; init; } = [];

        public class Item : BaseDto
        {
            public string Name { get; init; }
            public string Url { get; set; }
            public string? StorageUrl { get; init; }
            public string Extension { get; init; }
            public int Size { get; init; }
            public FileStatus StatusId { get; init; }
            public string StatusCode => EnumHelper<FileStatus>.GetLocalizedKey(StatusId);
            [JsonConverter(typeof(ZCodeJsonConverter))]
            public long FolderId { get; set; }
        }
    }

    public class Validator : AbstractValidator<Command>
    {
        public Validator()
        {
            RuleFor(v => v.Files)
                .NotNull()
                .NotEmpty();
        }
    }

    public class Handler(IMediator mediator,
        IStorageService storageService,
        IConfiguration configuration,
        IAppContext appContext,
        IFilePropertyBuilder propertyBuilder,
        ICurrentUser currentUser) : IRequestHandler<Command, ApiResult<Result>>
    {
        public async Task<ApiResult<Result>> Handle(Command command, CancellationToken cancellationToken)
        {
            var maxLength = configuration.GetValue("MaxFileSize", 5242880);
            if (command.Files.Any(c => c.Length > maxLength))
            {
                return new FailResult<Result>(ErrorMessages.FILE_TOO_BIG, HttpStatusCode.NotAcceptable, new Dictionary<string, string>
                {
                    { "size", $"{ Math.Round((decimal)maxLength/1024/1024, 0) }MB" },
                    { "fileNames", command.Files.Where(c => c.Length > maxLength).Select(c => c.FileName).Join(",") }
                });
            }

            List<Domain.Entities.File> files = [];
            var folderId = command.FolderId ?? await CreateRoot(cancellationToken);
            var existedFileNames = await appContext.Files
                .Where(c => c.FolderId == folderId)
                .Select(c => c.Name.ToLower())
                .ToListAsync(cancellationToken);

            var folder = await appContext.Folders.Where(x => x.Id == folderId)
                .AsNoTracking()
                .FirstOrDefaultAsync(cancellationToken);
            if (folder is null)
                return new FailResult<Result>(ErrorMessages.FOLDER_NOT_FOUND, HttpStatusCode.NotFound);

            var maxFileCountPerFolder = configuration.GetValue("MaxFileCountPerFolder", 999);
            if (existedFileNames.Count > maxFileCountPerFolder)
            {
                return new FailResult<Result>(ErrorMessages.MAX_FILES_PER_FOLDER_EXCEEDED, HttpStatusCode.NotAcceptable, new Dictionary<string, string>
                {
                    { "maxFileCountPerFolder", maxFileCountPerFolder.ToString() }
                });
            }

            var extensions = await appContext.FileExtensions
                .Where(c => !c.IsDisabled)
                .AsNoTracking()
                .ToListAsync(cancellationToken);

            foreach (var file in command.Files)
            {
                string extension = Path.GetExtension(file.FileName).ToLowerInvariant();
                var supportedExtension = await appContext.FileExtensions
                    .Where(c => !c.IsDisabled && c.Name.ToLower() == extension.ToLower())
                    .FirstOrDefaultAsync(cancellationToken);
                if (supportedExtension == null)
                {
                    return new FailResult<Result>(ErrorMessages.FILE_EXTENSION_NOT_SUPPORTED, HttpStatusCode.NotAcceptable, new Dictionary<string, string>
                    {
                        { "extension", extension }
                    });
                }
                var checkFileNameExistsLevel = configuration.GetValue("CheckFileNameExistsLevel", "None").ToUpperInvariant();
                if (checkFileNameExistsLevel == "FOLDER" && existedFileNames.Contains(file.FileName.ToLowerInvariant()))
                    return new FailResult<Result>(ErrorMessages.FILE_NAME_EXISTED, "name", file.FileName, HttpStatusCode.NotAcceptable);
                else if (checkFileNameExistsLevel == "ALL"
                    && await appContext.Files.Where(x => x.Name.ToLower() == file.FileName.ToLowerInvariant()).AnyAsync(cancellationToken))
                    return new FailResult<Result>(ErrorMessages.FILE_NAME_EXISTED, "name", file.FileName, HttpStatusCode.NotAcceptable);

                var fileName = GetFileName(file.FileName, existedFileNames, maxFileCountPerFolder);
                var newFile = new Domain.Entities.File
                {
                    FolderId = folderId,
                    Name = fileName,
                    Extension = extension,
                    Url = GetFileUrl(fileName),
                    TypeId = extensions.Find(x => x.Name.Equals(extension, StringComparison.OrdinalIgnoreCase))?.TypeId ?? FileType.None,
                    StatusId = FileStatus.Active
                };

                using var memoryStream = new MemoryStream();
                await file.CopyToAsync(memoryStream, cancellationToken);
                await storageService.UploadAsync(newFile, memoryStream, cancellationToken);
                newFile.Properties = propertyBuilder.GetProperties(newFile.Content, newFile.TypeId);
                files.Add(newFile);
            }

            appContext.Files.AddRange(files);
            await appContext.SaveChangesAsync(cancellationToken);

            await mediator.Publish(new OnFileUploadedEvent
            {
                FileIds = [.. files.Select(c => c.Id)],
                FolderId = folderId,
                FolderName = folder.Name
            }.SetCurrentUser(currentUser), cancellationToken);

            var result = new Result
            {
                UploadedFiles = [.. files.Select(c => new Result.Item
                {
                    Id = c.Id,
                    Url = c.Url,
                    Name = c.Name,
                    Extension = c.Extension,
                    FolderId = c.FolderId,
                    Size = c.Size,
                    StatusId = c.StatusId,
                    CreatedBy = c.CreatedBy,
                    CreatedTime = c.CreatedTime,
                    UpdatedBy = c.UpdatedBy ?? string.Empty,
                    UpdatedTime = c.UpdatedTime
                })]
            };

            return new SuccessResult<Result>(result);
        }

        private async Task<long> CreateRoot(CancellationToken cancellationToken)
        {
            var folder = await appContext.Folders.Where(c => c.Name == FolderConstants.RootFolderName).FirstOrDefaultAsync(cancellationToken);
            if (folder == null)
            {
                folder = new Folder
                {
                    Name = FolderConstants.RootFolderName
                };

                appContext.Folders.Add(folder);
                await appContext.SaveChangesAsync(cancellationToken);
            }
            return folder.Id;
        }

        private static string GetFileUrl(string fileName)
        {
            var extension = Path.GetExtension(fileName);
            var name = Path.GetFileNameWithoutExtension(fileName);
            return $"{StringHelper.GetUrlSEOCode(StringHelper.RemoveSign4VietnameseString(name))}_{IDGenerator.GenerateId()}{extension}";
        }

        private static string GetFileName(string fileName, List<string> existedFileNames, int maxFileCountPerFolder)
        {
            var newFileName = fileName;
            var extension = Path.GetExtension(newFileName);
            var name = Path.GetFileNameWithoutExtension(newFileName);
            if (name.Length > 72)
                name = name[..71];

            if (existedFileNames.Contains(fileName.ToLowerInvariant()))
            {
                for (var i = 1; i <= maxFileCountPerFolder; i++)
                {
                    var alterName = $"{name}-{i}{extension}";
                    if (!existedFileNames.Contains(alterName.ToLowerInvariant()))
                    {
                        newFileName = alterName;
                        break;
                    }
                }
            }
            existedFileNames.Add(newFileName.ToLowerInvariant());
            return newFileName;
        }
    }
}