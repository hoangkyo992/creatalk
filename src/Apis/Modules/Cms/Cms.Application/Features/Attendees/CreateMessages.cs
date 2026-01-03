namespace Cms.Application.Features.Attendees;

public class CreateMessages
{
    public record Command : IRequest<ApiResult<Result>>
    {
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

            var attendeeIds = await appContext.Attendees
                .Where(c => c.StatusId != AttendeeStatus.Cancelled)
                .Where(c => !c.Messages.Any(x => x.ProviderId == provider.Id))
                .Select(c => c.Id)
                .ToArrayAsync(cancellationToken);

            var chunks = attendeeIds.Chunk(100).ToList();
            foreach (var ids in chunks)
            {
                var messages = ids.Select(c => new AttendeeMessage
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
            }

            activityLogService.Setup(LogLabel.CreateAttendeeMessages, $"Create {attendeeIds.Length} {provider.Name} message(s)", currentUser);
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
                NumberOfAttendees = attendeeIds.Length
            });
        }
    }
}