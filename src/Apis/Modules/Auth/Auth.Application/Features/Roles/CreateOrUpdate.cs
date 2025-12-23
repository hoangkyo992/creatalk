namespace Auth.Application.Features.Roles;

public class CreateOrUpdate
{
    public record Command : IRequest<ApiResult<Result>>
    {
        [JsonIgnore]
        public long Id { get; set; }
        public string Name { get; init; }
        public string Description { get; init; }
    }

    public record Result
    {
        [JsonConverter(typeof(ZCodeJsonConverter))]
        public long Id { get; init; }
    }

    public class Validator : AbstractValidator<Command>
    {
        public Validator()
        {
            RuleFor(v => v.Name)
                .NotNull()
                .NotEmpty();
            RuleFor(v => v.Description)
                .NotNull()
                .NotEmpty();
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
                .Where(c => c.Id != command.Id && c.Name.ToLower() == command.Name.ToLower())
                .FirstOrDefaultAsync(cancellationToken);
            if (item != null)
                return new FailResult<Result>(ErrorMessages.ROLE_EXISTED, "name", command.Name, HttpStatusCode.Conflict);

            item = await context.Roles
                .Where(c => c.Id != (int)SystemRole.SuperAdmin)
                .Where(c => c.Id == command.Id)
                .FirstOrDefaultAsync(cancellationToken);
            if (item == null && command.Id > 0)
                return new FailResult<Result>(ErrorMessages.ROLE_NOT_FOUND, HttpStatusCode.NotFound);

            var oldValue = new object();
            var action = ActivityLogAction.Create;

            ApplicationEvent? evt = null;
            if (item == null)
            {
                activityLogService.Setup(LogLabel.CreateUserRole, $"Role [{command.Name}] is added", currentUser);

                item = new Role
                {
                    Id = IDGenerator.GenerateId(),
                    CreatedTime = DateTime.UtcNow,
                    CreatedBy = currentUser.Username,
                };
                context.Roles.Add(item);

                evt = new RoleEvents.OnDataCreatedEvent
                {
                    Ids = [item.Id]
                };
            }
            else
            {
                activityLogService.Setup(LogLabel.UpdateUserRole, $"Role [{item.Name}] is updated", currentUser);
                oldValue = mapper.Map<RoleLoggingDto>(item);
                action = ActivityLogAction.Update;

                item.UpdatedTime = DateTime.UtcNow;
                item.UpdatedBy = currentUser.Username;

                evt = new RoleEvents.OnDataUpdatedEvent
                {
                    Ids = [item.Id]
                };
            }

            item.Name = command.Name;
            item.Description = command.Description;

            activityLogService.AddLog(new LogEntityModel(nameof(Role), item.Id)
            {
                Action = action,
                OldValue = oldValue,
                NewValue = mapper.Map<RoleLoggingDto>(item)
            });

            await context.SaveChangesAsync(cancellationToken);
            await mediator.Publish(evt.SetCurrentUser(currentUser), cancellationToken);

            await activityLogService.CommitAsync();

            return new SuccessResult<Result>(new Result
            {
                Id = item.Id
            });
        }
    }
}