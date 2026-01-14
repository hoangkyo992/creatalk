namespace Cms.Application.Features.Attendees;

public class ResendMessage
{
    public record Command : IRequest<ApiResult<Result>>
    {
        [JsonIgnore]
        public long Id { get; set; }
        public string ProviderCode { get; init; }
    }

    public record Result
    {
        public int NumberOfAttendees { get; init; }
    }

    public class Validator : AbstractValidator<Command>
    {
        public Validator()
        {
            RuleFor(v => v.Id)
                .NotNull()
                .GreaterThan(0);
            RuleFor(v => v.ProviderCode)
                .NotNull()
                .NotEmpty();
        }
    }

    public class Handler(IAppContext appContext,
        IActivityLogService activityLogService,
        IQueueService queueService,
        ICurrentUser currentUser) : IRequestHandler<Command, ApiResult<Result>>
    {
        public async Task<ApiResult<Result>> Handle(Command command, CancellationToken cancellationToken)
        {
            var provider = await appContext.MessageProviders
                .Where(c => !c.IsDisabled)
                .Where(c => c.Code == command.ProviderCode)
                .AsNoTracking()
                .FirstOrDefaultAsync(cancellationToken);

            if (provider == null)
                return new FailResult<Result>(ErrorMessages.MESSAGE_PROVIDER_NOT_FOUND, HttpStatusCode.NotFound);

            var attendee = await appContext.Attendees
                .Include(c => c.Messages)
                .Where(c => c.Id == command.Id)
                .FirstOrDefaultAsync();
            if (attendee == null)
                return new FailResult<Result>(ErrorMessages.ATTENDEE_NOT_FOUND, HttpStatusCode.NotFound);

            attendee.Messages.ForEach(x => x.IsDeleted = true);

            var attendeeIds = new List<long> { command.Id };
            var messages = attendeeIds.Select(c => new AttendeeMessage
            {
                AttendeeId = c,
                ProviderId = provider.Id,
                RequestPayload = string.Empty,
                ResponsePayload = string.Empty,
                MessageId = string.Empty,
                StatusId = MessageStatus.New
            }).ToList();
            appContext.AttendeeMessages.AddRange(messages);
            await appContext.SaveChangesAsync(cancellationToken);
            await messages.ForEachAsync(async c => await queueService.EnqueueAsync(c.Id));

            activityLogService.Setup(LogLabel.CreateAttendeeMessages, $"Create {attendeeIds.Count} {provider.Name} message(s)", currentUser);
            activityLogService.AddLog(new LogEntityModel(nameof(MessageProvider), provider.Id)
            {
                Action = ActivityLogAction.Create,
                OldValue = new { },
                NewValue = new
                {
                    attendeeIds
                }
            });

            await activityLogService.CommitAsync();

            return new SuccessResult<Result>(new Result
            {
                NumberOfAttendees = attendeeIds.Count
            });
        }
    }
}