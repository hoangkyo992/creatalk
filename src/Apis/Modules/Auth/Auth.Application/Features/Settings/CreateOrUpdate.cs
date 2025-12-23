namespace Auth.Application.Features.Settings;

public class CreateOrUpdate
{
    public record Command : IRequest<ApiResult<Result>>
    {
        [JsonIgnore]
        public long Id { get; set; }
        public string Key { get; init; }
        public string Value { get; init; }
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
            RuleFor(v => v.Key)
                .NotNull()
                .NotEmpty();
            RuleFor(v => v.Value)
                .NotNull()
                .NotEmpty();

            // Validate value
            When(v => v.Key == SettingKeys.General, () =>
            {
            });
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
            var item = await context.Settings
                .Where(c => c.Id != command.Id && c.Key.ToLower() == command.Key.ToLower())
                .FirstOrDefaultAsync(cancellationToken);
            if (item != null)
                return new FailResult<Result>(ErrorMessages.SETTING_EXISTED, "key", command.Key, HttpStatusCode.Conflict);

            item = await context.Settings
                .Where(c => c.Id == command.Id)
                .FirstOrDefaultAsync(cancellationToken);
            if (item == null && command.Id > 0)
                return new FailResult<Result>(ErrorMessages.SETTING_NOT_FOUND, HttpStatusCode.NotFound);

            var oldValue = new object();
            var action = ActivityLogAction.Create;

            ApplicationEvent? evt = null;
            if (item == null)
            {
                activityLogService.Setup(LogLabel.CreateSetting, $"Setting [{command.Key}] is added", currentUser);

                item = new Setting
                {
                    Id = IDGenerator.GenerateId(),
                    CreatedTime = DateTime.UtcNow,
                    CreatedBy = currentUser.Username,
                };
                context.Settings.Add(item);

                evt = new SettingEvents.OnDataCreatedEvent
                {
                    Ids = [item.Id]
                };
            }
            else
            {
                activityLogService.Setup(LogLabel.UpdateSetting, $"Setting [{item.Key}] is updated", currentUser);
                oldValue = mapper.Map<SettingLoggingDto>(item);
                action = ActivityLogAction.Update;

                item.UpdatedTime = DateTime.UtcNow;
                item.UpdatedBy = currentUser.Username;

                evt = new SettingEvents.OnDataUpdatedEvent
                {
                    Ids = [item.Id]
                };
            }

            item.Key = command.Key;
            item.Value = command.Value;

            activityLogService.AddLog(new LogEntityModel(nameof(Setting), item.Id)
            {
                Action = action,
                OldValue = oldValue,
                NewValue = mapper.Map<SettingLoggingDto>(item)
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