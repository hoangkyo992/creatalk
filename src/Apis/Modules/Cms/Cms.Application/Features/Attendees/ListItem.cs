namespace Cms.Application.Features.Attendees;

public class ListItem
{
    public record Request : DataSourceRequest, IRequest<ApiResult<Result>>
    {
    }

    public record Result : DataSourceResult<Result.Item>
    {
        public class Item : BaseDto
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

        public Result(DataSourceResult<Item> result) : base(result)
        {
        }
    }

    public class Validator : AbstractValidator<Request>
    {
        public Validator()
        {
        }
    }

    public class Handler(IAppContext appContext) : IRequestHandler<Request, ApiResult<Result>>
    {
        public async Task<ApiResult<Result>> Handle(Request request, CancellationToken cancellationToken)
        {
            var data = await appContext.Attendees
                .SelectWithDefaultOrder(c => new Result.Item
                {
                    Id = c.Id,
                    FirstName = c.FirstName,
                    LastName = c.LastName,
                    Email = c.Email,
                    PhoneNumber = c.PhoneNumber,
                    TicketId = c.TicketId,
                    TicketNumber = c.TicketNumber,
                    Messages = c.Messages
                        .Select(m => new Result.MessageItem
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
                }).ToDataSourceResultAsync(request, cancellationToken);

            return new SuccessResult<Result>(new Result(data));
        }
    }
}