namespace Auth.Application.Features.Roles;

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
        IAppContext context,
        IMapper mapper,
        IActivityLogService activityLogService,
        ICurrentUser currentUser) : IRequestHandler<Command, ApiResult<Result>>
    {
        public async Task<ApiResult<Result>> Handle(Command command, CancellationToken cancellationToken)
        {
            var item = await context.Roles
                .Where(c => c.Id != (int)SystemRole.SuperAdmin)
                .Where(c => c.Id == command.Id)
                .Include(c => c.Users)
                .FirstOrDefaultAsync(cancellationToken);
            if (item == null)
                return new FailResult<Result>(ErrorMessages.ROLE_NOT_FOUND, HttpStatusCode.NotFound);
            if (item.Users.Count != 0)
            {
                return new FailResult<Result>(ErrorMessages.ROLE_HAS_USERS, HttpStatusCode.NotAcceptable);
            }
            activityLogService.Setup(LogLabel.DeleteUserRole, $"Role [{item.Name}] is deleted", currentUser);
            var oldValue = mapper.Map<RoleLoggingDto>(item);

            item.IsDeleted = true;
            item.UpdatedBy = currentUser.Username;
            item.UpdatedTime = DateTime.UtcNow;

            activityLogService.AddLog(new LogEntityModel(nameof(Role), item.Id)
            {
                Action = ActivityLogAction.Delete,
                OldValue = oldValue,
                NewValue = mapper.Map<RoleLoggingDto>(item)
            });

            await context.SaveChangesAsync(cancellationToken);
            await mediator.Publish(new RoleEvents.OnDataDeletedEvent
            {
                Ids = [command.Id]
            }.SetCurrentUser(currentUser), cancellationToken);

            await activityLogService.CommitAsync();

            return new SuccessResult<Result>(new Result());
        }
    }
}