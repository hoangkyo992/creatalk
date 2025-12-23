namespace Auth.Application.Features.Roles;

public class UpdateFeatures
{
    public record Command : IRequest<ApiResult<Result>>
    {
        [JsonIgnore]
        public long Id { get; set; }
        public List<Feature> Features { get; init; }
        public class Feature
        {
            public string Name { get; set; }
            public string Action { get; set; }

            public override string ToString()
            {
                return $"{Name.ToLower()}__{Action.ToLower()}";
            }
        }
    }

    public record Result
    {
        public int TotalUpdated { get; set; }
    }

    public class Validator : AbstractValidator<Command>
    {
        public Validator()
        {
            RuleFor(v => v.Id)
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
                .Include(c => c.Features)
                .FirstOrDefaultAsync(cancellationToken);
            if (item == null)
                return new FailResult<Result>(ErrorMessages.ROLE_NOT_FOUND, HttpStatusCode.NotFound);

            var addingItems = command.Features.Where(c => item.Features.FirstOrDefault(x => x.ToString() == c.ToString()) == null);
            var removingItems = item.Features.Where(c => command.Features.Find(x => x.ToString() == c.ToString()) == null);

            activityLogService.Setup(LogLabel.UpdateUserRoleFeatures, $"Role [{item.Name}]'s features is updated", currentUser);

            activityLogService.AddLogs(removingItems.Select(c => new LogEntityModel(nameof(RoleFeature), c.Id)
            {
                Action = ActivityLogAction.Delete,
                OldValue = mapper.Map<RoleFeatureLoggingDto>(c, (opt) => opt.AfterMap((src, dest) => dest.RoleName = item.Name)),
                NewValue = new { }
            }));

            context.RoleFeatures.RemoveRange(removingItems);

            foreach (var feat in addingItems)
            {
                var newItem = new RoleFeature
                {
                    Id = IDGenerator.GenerateId(),
                    CreatedTime = DateTime.UtcNow,
                    CreatedBy = currentUser.Username,
                    RoleId = command.Id,
                    Feature = feat.Name.ToUpper(),
                    Action = feat.Action.ToUpper()
                };
                context.RoleFeatures.Add(newItem);

                var newValue = mapper.Map<RoleFeatureLoggingDto>(newItem);
                newValue.RoleName = item.Name;
                activityLogService.AddLog(new LogEntityModel(nameof(RoleFeature), newItem.Id)
                {
                    Action = ActivityLogAction.Create,
                    OldValue = new { },
                    NewValue = newValue
                });
            }

            var total = await context.SaveChangesAsync(cancellationToken);

            await mediator.Publish(new RoleFeatureEvents.OnDataUpdatedEvent
            {
                Ids = [item.Id]
            }.SetCurrentUser(currentUser), cancellationToken);

            await activityLogService.CommitAsync();

            return new SuccessResult<Result>(new Result
            {
                TotalUpdated = total
            });
        }
    }
}