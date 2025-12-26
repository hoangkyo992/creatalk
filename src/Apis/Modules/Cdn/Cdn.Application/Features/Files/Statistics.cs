namespace Cdn.Application.Features.Files;

public class Statistics
{
    public record Request : IRequest<ApiResult<Result>>
    {
    }

    public class Result
    {
        public List<FolderItem> Folders { get; set; } = [];
    }

    public class FolderItem
    {
        public string Name { get; init; }
        public Dictionary<FileType, int> FileTypes { get; set; } = [];
    }

    public class Validator : AbstractValidator<Request>
    {
        public Validator()
        {
        }
    }

    public class Handler(IAppContext appContext) : IRequestHandler<Request, ApiResult<Result>>
    {
        public async Task<ApiResult<Result>> Handle(Request request, CancellationToken cancellationToken)
        {
            var data = (await appContext.Files
                .Where(c => c.StatusId == FileStatus.Active)
                .Select(c => new
                {
                    c.Id,
                    FolderName = c.Folder.Name,
                    c.FolderId,
                    c.TypeId
                })
                .GroupBy(c => new { c.FolderId, c.FolderName })
                .ToListAsync(cancellationToken))
                .Select(c => new FolderItem
                {
                    Name = c.Key.FolderName,
                    FileTypes = c.GroupBy(cc => cc.TypeId)
                    .ToDictionary(cc => cc.Key, cc => cc.Count())
                })
                .ToList();

            data.ForEach(c =>
            {
                c.FileTypes = EnumHelper<FileType>.GetValues()
                    .ToDictionary(cc => cc, cc => c.FileTypes.GetValueOrDefault(cc));
            });

            return new SuccessResult<Result>(new Result
            {
                Folders = data
            });
        }
    }
}