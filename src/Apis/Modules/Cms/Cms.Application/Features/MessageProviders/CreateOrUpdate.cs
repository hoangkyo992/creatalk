
namespace Cms.Application.Features.MessageProviders;

public class CreateOrUpdate
{
    public record Command : IRequest<ApiResult<Result>>
    {
        [JsonIgnore]
        public long Id { get; set; }
        public string Code { get; init; }
        public string Name { get; init; }
        public string Settings { get; init; }
        public bool IsDisabled { get; init; }
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
            RuleFor(v => v.Code)
                .NotNull()
                .NotEmpty();
            RuleFor(v => v.Name)
                .NotNull()
                .NotEmpty();
            RuleFor(v => v.Settings)
                .NotNull()
                .NotEmpty();
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
            var item = await appContext.MessageProviders
                .Where(c => c.Id != command.Id && c.Code.ToLower() == command.Code.ToLower())
                .FirstOrDefaultAsync(cancellationToken);
            if (item != null)
                return new FailResult<Result>(ErrorMessages.MESSAGE_PROVIDER_EXISTED, "code", command.Code, HttpStatusCode.Conflict);

            item = await appContext.MessageProviders
                .Where(c => c.Id == command.Id)
                .FirstOrDefaultAsync(cancellationToken);
            if (item == null && command.Id > 0)
                return new FailResult<Result>(ErrorMessages.MESSAGE_PROVIDER_NOT_FOUND, HttpStatusCode.NotFound);

            var oldValue = new object();
            var action = ActivityLogAction.Create;

            ApplicationEvent? evt = null;
            if (item == null)
            {
                activityLogService.Setup(LogLabel.CreateMessageProvider, $"Message provider [{command.Code}] is created", currentUser);

                item = new()
                {
                };
                appContext.MessageProviders.Add(item);
                evt = new OnMessageProviderCreatedEvent
                {
                    Ids = [item.Id]
                };
            }
            else
            {
                activityLogService.Setup(LogLabel.UpdateMessageProvider, $"MessageProvider [{item.Code}] is updated", currentUser);
                oldValue = mapper.Map<MessageProviderLoggingDto>(item);
                action = ActivityLogAction.Update;
                evt = new OnMessageProviderUpdatedEvent
                {
                    Ids = [item.Id]
                };
            }

            item.Code = command.Code;
            item.Name = command.Name;
            item.Settings = command.Settings;
            item.IsDisabled = command.IsDisabled;

            activityLogService.AddLog(new LogEntityModel(nameof(MessageProvider), item.Id)
            {
                Action = action,
                OldValue = oldValue,
                NewValue = mapper.Map<MessageProviderLoggingDto>(item)
            });

            await appContext.SaveChangesAsync(cancellationToken);
            await mediator.Publish(evt.SetCurrentUser(currentUser), cancellationToken);
            await activityLogService.CommitAsync();

            return new SuccessResult<Result>(new Result
            {
                Id = item.Id
            });
        }
    }
}