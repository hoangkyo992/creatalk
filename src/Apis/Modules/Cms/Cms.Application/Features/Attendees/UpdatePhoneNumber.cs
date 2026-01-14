namespace Cms.Application.Features.Attendees;

public class UpdatePhoneNumber
{
    public record Command : IRequest<ApiResult<Result>>
    {
        [JsonIgnore]
        public long Id { get; set; }

        public string PhoneNumber { get; init; }
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
            RuleFor(v => v.PhoneNumber)
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
            var item = await appContext.Attendees
                .Where(c => c.Id == command.Id)
                .FirstOrDefaultAsync(cancellationToken);
            if (item == null)
                return new FailResult<Result>(ErrorMessages.ATTENDEE_NOT_FOUND, HttpStatusCode.NotFound);

            activityLogService.Setup(LogLabel.UpdateAttendee, $"Attendee [{item.PhoneNumber}] is updated", currentUser);
            var oldValue = mapper.Map<AttendeeLoggingDto>(item);

            item.PhoneNumber = command.PhoneNumber;

            activityLogService.AddLog(new LogEntityModel(nameof(Attendee), item.Id)
            {
                Action = ActivityLogAction.Update,
                OldValue = oldValue,
                NewValue = mapper.Map<AttendeeLoggingDto>(item)
            });

            await appContext.SaveChangesAsync(cancellationToken);
            await mediator.Publish(new OnAttendeeUpdatedEvent
            {
                Ids = [command.Id],
            }.SetCurrentUser(currentUser), cancellationToken);

            await activityLogService.CommitAsync();

            return new SuccessResult<Result>(new Result());
        }
    }
}