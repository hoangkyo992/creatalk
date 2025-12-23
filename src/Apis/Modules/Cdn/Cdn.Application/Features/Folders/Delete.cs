using AutoMapper;
using Cdn.Domain.Shared.Constants;
using Common.Domain.Enums;

namespace Cdn.Application.Features.Folders;

public class Delete
{
    public record Command : IRequest<ApiResult<Result>>
    {
        [JsonIgnore]
        public long Id { get; set; }
    }

    public record Result
    {
    }

    public class Validator : AbstractValidator<Command>
    {
        public Validator()
        {
            RuleFor(v => v.Id)
                .NotNull()
                .GreaterThan(0);
        }
    }

    public class Handler(IMediator mediator,
        IAppContext appContext,
        IMapper mapper,
        IActivityLogService activityLogService,
        ICurrentUser currentUser) : IRequestHandler<Command, ApiResult<Result>>
    {
        public async Task<ApiResult<Result>> Handle(Command command, CancellationToken cancellationToken)
        {
            var item = await appContext.Folders
                .Where(c => c.Id == command.Id)
                .Where(c => c.Name != FolderConstants.RootFolderName)
                .Where(c => c.StatusId == FolderStatus.IsInTrash)
                .FirstOrDefaultAsync(cancellationToken);
            if (item == null)
                return new FailResult<Result>(ErrorMessages.FOLDER_NOT_FOUND, HttpStatusCode.NotFound);

            activityLogService.Setup(LogLabel.DeleteFolder, $"Folder [{item.Name}] is deleted", currentUser);
            var oldValue = mapper.Map<FolderLoggingDto>(item);

            item.IsDeleted = true;

            var ids = await DeleteSubFolders([item.Id], cancellationToken);

            activityLogService.AddLog(new LogEntityModel(nameof(Folder), item.Id)
            {
                Action = ActivityLogAction.Delete,
                OldValue = oldValue,
                NewValue = new { }
            });

            await appContext.SaveChangesAsync(cancellationToken);
            await mediator.Publish(new OnFolderDeletedEvent
            {
                Ids = ids.Distinct().ToList()
            }.SetCurrentUser(currentUser), cancellationToken);

            await activityLogService.CommitAsync();

            return new SuccessResult<Result>(new Result());
        }

        private async Task<List<long>> DeleteSubFolders(List<long> ids, CancellationToken cancellationToken)
        {
            var folders = await appContext.Folders
                .Where(c => c.ParentId > 0 && ids.Contains(c.ParentId.Value))
                .ToListAsync(cancellationToken);
            if (folders.Count > 0)
            {
                folders.ForEach(c =>
                {
                    var oldValue = mapper.Map<FolderLoggingDto>(c);

                    c.IsDeleted = true;
                    c.UpdatedBy = currentUser.Username;
                    c.UpdatedTime = DateTime.UtcNow;

                    activityLogService.AddLog(new LogEntityModel(nameof(Folder), c.Id)
                    {
                        Action = ActivityLogAction.Delete,
                        OldValue = oldValue,
                        Description = $"Folder [{c.Name}] is deleted",
                        NewValue = new { }
                    });
                });
                var subIds = await DeleteSubFolders(folders.Select(c => c.Id).ToList(), cancellationToken);
                return ids.Union(subIds).Distinct().ToList();
            }
            return ids;
        }
    }
}