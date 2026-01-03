using Cdn.Application.Shared.Configurations;

namespace Cms.Application.Features.Attendees;

public class ListItem
{
    public record Request : DataSourceRequest, IRequest<ApiResult<Result>>
    {
        public string? ProviderCode { get; init; }
        public bool IsSent { get; init; }
        public string? MessageStatusIds { get; init; }
        public AttendeeStatus? StatusId { get; init; }
        public DateTime? StartTime { get; init; }
        public DateTime? EndTime { get; init; }
    }

    public record Result : DataSourceResult<Result.Item>
    {
        public class Item : BaseDto
        {
            public string FirstName { get; init; }
            public string LastName { get; init; }
            public string FullName { get; set; }
            public string Email { get; init; }
            public string PhoneNumber { get; init; }
            public string TicketNumber { get; init; }
            public AttendeeStatus StatusId { get; init; }

            [JsonConverter(typeof(ZCodeJsonConverter))]
            public long TicketId { get; init; }
            public string TicketUrl { get; set; }
            public string TicketZone { get; set; }

            public IEnumerable<MessageItem> Messages { get; init; } = [];
        }

        public class MessageItem : BaseDto
        {
            public string ProviderCode { get; init; }
            public string ProviderName { get; init; }
            public MessageStatus StatusId { get; init; }
            public DateTime? SentAt { get; init; }
            public DateTime? UserReceivedAt { get; init; }
            public string? EventPayload { get; init; }
            public string MessageId { get; init; }
            public string RequestPayload { get; init; }
            public string ResponsePayload { get; init; }
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

    public class Handler(IAppContext appContext, IOptions<CdnServerConfiguration> options) : IRequestHandler<Request, ApiResult<Result>>
    {
        public async Task<ApiResult<Result>> Handle(Request request, CancellationToken cancellationToken)
        {
            var q = appContext.Attendees
                .WhereIf(request.StatusId.HasValue, c => c.StatusId == request.StatusId)
                .WhereIf(request.StartTime.HasValue, c => c.CreatedTime >= request.StartTime)
                .WhereIf(request.EndTime.HasValue, c => c.CreatedTime <= request.EndTime);
            if (!string.IsNullOrWhiteSpace(request.Query))
            {
                var keyword = request.Query.Trim().ToLower();
                q = q.Where(c => c.FullName.ToLower().Contains(keyword)
                    || c.TicketNumber.ToLower().Contains(keyword)
                    || c.Email.ToLower().Contains(keyword)
                    || c.PhoneNumber.ToLower().Contains(keyword)
                    || c.CreatedBy.ToLower().Contains(keyword)
                );
            }

            if (!string.IsNullOrWhiteSpace(request.ProviderCode))
            {
                if (request.IsSent)
                {
                    if (!string.IsNullOrWhiteSpace(request.MessageStatusIds))
                    {
                        var messageStatusIds = EnumHelper<MessageStatus>.GetEnumValuesFromString(request.MessageStatusIds);
                        q = q.Where(c => c.Messages.Any(x => x.Provider.Code == request.ProviderCode && messageStatusIds.Contains(x.StatusId)));
                    }
                    else
                    {
                        q = q.Where(c => c.Messages.Any(x => x.Provider.Code == request.ProviderCode));
                    }
                }
                else
                {
                    q = q.Where(c => !c.Messages.Any(x => x.Provider.Code == request.ProviderCode));
                }
            }

            var data = await q
                .SelectWithDefaultOrder(c => new Result.Item
                {
                    Id = c.Id,
                    FirstName = c.FirstName,
                    LastName = c.LastName,
                    FullName = c.FullName,
                    Email = c.Email,
                    PhoneNumber = c.PhoneNumber,
                    TicketId = c.TicketId,
                    TicketNumber = c.TicketNumber,
                    StatusId = c.StatusId,
                    TicketZone = c.TicketZone,
                    Messages = c.Messages
                        .Select(m => new Result.MessageItem
                        {
                            Id = m.Id,
                            ProviderCode = m.Provider.Code,
                            ProviderName = m.Provider.Name,
                            SentAt = m.SentAt,
                            UserReceivedAt = m.UserReceivedAt,
                            EventPayload = m.EventPayload,
                            MessageId = m.MessageId,
                            RequestPayload = m.RequestPayload,
                            ResponsePayload = m.ResponsePayload,
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

            data.Data.ForEach(v =>
            {
                if (v.TicketId > 0)
                    v.TicketUrl = $"{options.Value.PublicPath}/files/{ZCode.Get(v.TicketId)}";
            });

            return new SuccessResult<Result>(new Result(data));
        }
    }
}