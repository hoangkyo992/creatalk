namespace Cms.Application.Features.Attendees;

public class GetItem
{
    public record Request : IRequest<ApiResult<Result>>
    {
        [JsonConverter(typeof(ZCodeJsonConverter))]
        public long Id { get; init; }
    }

    public class Result : BaseDto
    {
        public string FirstName { get; init; }
        public string LastName { get; init; }
        public string FullName => $"{LastName} {FirstName}";
        public string Email { get; init; }
        public string PhoneNumber { get; init; }
        public string TicketNumber { get; init; }

        [JsonConverter(typeof(ZCodeJsonConverter))]
        public long TicketId { get; init; }

        public IEnumerable<MessageItem> Messages { get; init; } = [];
    }

    public class MessageItem : BaseDto
    {
        public string ProviderCode { get; init; }
        public string ProviderName { get; init; }
        public int StatusId { get; init; }
    }

    public class Validator : AbstractValidator<Request>
    {
        public Validator()
        {
            RuleFor(v => v.Id)
                .NotNull()
                .GreaterThan(0);
        }
    }

    public class Handler(IAppContext appContext) : IRequestHandler<Request, ApiResult<Result>>
    {
        public async Task<ApiResult<Result>> Handle(Request request, CancellationToken cancellationToken)
        {
            var item = await appContext.Attendees
                .Where(c => c.Id == request.Id)
                .Select(c => new Result
                {
                    Id = c.Id,
                    FirstName = c.FirstName,
                    LastName = c.LastName,
                    Email = c.Email,
                    PhoneNumber = c.PhoneNumber,
                    TicketId = c.TicketId,
                    TicketNumber = c.TicketNumber,
                    Messages = c.Messages
                        .Select(m => new MessageItem
                        {
                            Id = m.Id,
                            ProviderCode = m.Provider.Code,
                            ProviderName = m.Provider.Name,
                            StatusId = m.StatusId,
                            CreatedBy = m.CreatedBy,
                            CreatedTime = m.CreatedTime,
                            UpdatedBy = m.UpdatedBy,
                            UpdatedTime = m.UpdatedTime
                        }),
                    CreatedBy = c.CreatedBy,
                    CreatedTime = c.CreatedTime,
                    UpdatedBy = c.UpdatedBy,
                    UpdatedTime = c.UpdatedTime
                }).FirstOrDefaultAsync(cancellationToken);

            return item == null
                ? new FailResult<Result>(ErrorMessages.ATTENDEE_NOT_FOUND, HttpStatusCode.NotFound)
                : new SuccessResult<Result>(item);
        }
    }
}