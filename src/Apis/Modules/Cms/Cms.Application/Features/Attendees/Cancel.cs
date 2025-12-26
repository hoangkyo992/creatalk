namespace Cms.Application.Features.Attendees;

public class Cancel
{
    public record Command : IRequest<ApiResult<Result>>
    {
        [JsonIgnore]
        public long Id { get; set; }

        public bool IsCancelled { get; init; }
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
            var item = await appContext.Attendees
                .Where(c => c.Id == command.Id)
                .FirstOrDefaultAsync(cancellationToken);
            if (item == null)
                return new FailResult<Result>(ErrorMessages.ATTENDEE_NOT_FOUND, HttpStatusCode.NotFound);

            if (command.IsCancelled)
            {
                if (item.StatusId == AttendeeStatus.Cancelled)
                    return new FailResult<Result>(ErrorMessages.ATTENDEE_CANCELLED, HttpStatusCode.NotAcceptable);
                activityLogService.Setup(LogLabel.CancelAttendee, $"Attendee [{item.Email}] is cancelled", currentUser);
            }
            else
            {
                if (item.StatusId != AttendeeStatus.Cancelled)
                    return new FailResult<Result>(ErrorMessages.ATTENDEE_NOT_CANCELLED, HttpStatusCode.NotAcceptable);
                activityLogService.Setup(LogLabel.RestoreAttendee, $"Attendee [{item.Email}] is restored", currentUser);
            }
            var oldValue = mapper.Map<AttendeeLoggingDto>(item);

            item.StatusId = command.IsCancelled ? AttendeeStatus.Cancelled : AttendeeStatus.Default;

            activityLogService.AddLog(new LogEntityModel(nameof(Attendee), item.Id)
            {
                Action = ActivityLogAction.Update,
                OldValue = oldValue,
                NewValue = mapper.Map<AttendeeLoggingDto>(item)
            });

            await appContext.SaveChangesAsync(cancellationToken);
            await mediator.Publish(new OnAttendeeCancelledEvent
            {
                Ids = [command.Id],
                IsCancelled = command.IsCancelled
            }.SetCurrentUser(currentUser), cancellationToken);

            await activityLogService.CommitAsync();

            return new SuccessResult<Result>(new Result());
        }
    }
}