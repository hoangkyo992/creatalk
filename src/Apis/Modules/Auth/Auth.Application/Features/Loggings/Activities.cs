namespace Auth.Application.Features.Loggings;

public class Activities
{
    public record Request : DataSourceRequest, IRequest<ApiResult<Result>>
    {
        public DateTime StartTime { get; init; }
        public DateTime EndTime { get; init; }
        public string Username { get; init; }
        public long? UserId { get; init; }
    }

    public record Result : DataSourceResult<Result.Item>
    {
        public class Item : IdDto
        {
            [JsonConverter(typeof(ZCodeJsonConverter))]
            public long UserId { get; init; }
            public string Label { get; init; }
            public string Username { get; init; }
            public DateTime Time { get; init; }
            public string IpAddress { get; init; }
            public string Source { get; init; }
            public string MethodName { get; init; }
            public string Action { get; init; }
            public string Description { get; init; }
            public string RequestId { get; init; }
            public List<LogEntityItem> LogEntities { get; set; }
        }

        public class LogEntityItem : IdDto
        {
            [JsonIgnore]
            public long ActivityId { get; init; }
            public string EntityName { get; init; }
            public string Pk { get; init; }
            public char CRUD { get; init; }
            public DateTime Time { get; init; }
            public string Description { get; init; }
            public string OldValue { get; init; }
            public string NewValue { get; init; }
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

    public class Handler(IAppContext appContext, ICurrentUser currentUser) : IRequestHandler<Request, ApiResult<Result>>
    {
        public async Task<ApiResult<Result>> Handle(Request request, CancellationToken cancellationToken)
        {
            var data = await appContext.LogActivities
                .Where(c => c.Time >= request.StartTime && c.Time <= request.EndTime)
                .WhereIf(request.UserId > 0, c => c.UserId == request.UserId)
                .WhereIf(!currentUser.IsSystemUser, c => c.UserId > int.MaxValue)
                .WhereIf(!string.IsNullOrWhiteSpace(request.Username), c => c.Username == request.Username)
                .OrderByDescending(c => c.Id)
                .Select(c => new Result.Item
                {
                    Id = c.Id,
                    Time = c.Time,
                    UserId = c.UserId,
                    Username = c.Username,
                    Action = c.Action,
                    IpAddress = c.IpAddress,
                    Description = c.Description,
                    Label = c.Label,
                    MethodName = c.MethodName,
                    RequestId = c.RequestId,
                    Source = c.Source
                }).ToDataSourceResultAsync(request, cancellationToken);

            var logIds = data.Data.Select(c => c.Id).ToList();
            if (logIds.Count > 0)
            {
                var entities = await appContext.LogEntities
                    .Where(c => logIds.Contains(c.ActivityId))
                    .Select(c => new Result.LogEntityItem
                    {
                        Id = c.Id,
                        ActivityId = c.ActivityId,
                        CRUD = c.CRUD,
                        Pk = c.Pk,
                        Description = c.Description,
                        EntityName = c.EntityName,
                        NewValue = c.NewValue,
                        OldValue = c.OldValue,
                        Time = c.Time
                    })
                    .ToListAsync(cancellationToken);
                foreach (var item in data.Data)
                {
                    item.LogEntities = entities.Where(c => c.ActivityId == item.Id).ToList();
                }
            }

            return new SuccessResult<Result>(new Result(data));
        }
    }
}