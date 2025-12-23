namespace Auth.Application.Features.Users;

public class GetUserSessions
{
    public record Request : DataSourceRequest, IRequest<ApiResult<Result>>
    {
        public DateTime StartTime { get; init; }
        public DateTime EndTime { get; init; }
        public bool ActiveSessionOnly { get; init; }
        [JsonConverter(typeof(ZCodeJsonConverter))]
        public long? UserId { get; init; }
    }

    public record Result : DataSourceResult<Result.Item>
    {
        public class Item : BaseDto
        {
            public DateTime StartTime { get; init; }
            public DateTime? EndTime { get; init; }
            public string EndBy { get; init; }
            public string IpAddress { get; init; }
            public string Platform { get; init; }
            public string Navigator { get; init; }
            [JsonConverter(typeof(ZCodeJsonConverter))]
            public long UserId { get; init; }
            public string Username { get; init; }
        }

        public Result(DataSourceResult<Item> result)
        {
            Data = result.Data;
            Total = result.Total;
            AdditionalData = result.AdditionalData;
        }
    }

    public class Validator : AbstractValidator<Request>
    {
        public Validator()
        {
        }
    }

    public class Handler(IAppContext context, ICurrentUser currentUser) : IRequestHandler<Request, ApiResult<Result>>
    {
        public async Task<ApiResult<Result>> Handle(Request request, CancellationToken cancellationToken)
        {
            var data = await context.UserSessions
                .Where(c => c.CreatedTime >= request.StartTime && c.CreatedTime <= request.EndTime)
                .WhereIf(!currentUser.IsSystemUser, c => c.UserId > int.MaxValue)
                .WhereIf(request.ActiveSessionOnly, c => c.EndTime == null)
                .WhereIf(request.UserId > 0, c => c.UserId == request.UserId)
                .SelectWithDefaultOrder(c => new Result.Item
                {
                    Id = c.Id,
                    EndTime = c.EndTime,
                    StartTime = c.StartTime,
                    EndBy = c.EndBy ?? string.Empty,
                    IpAddress = c.IpAddress ?? string.Empty,
                    Navigator = c.Navigator ?? string.Empty,
                    Platform = c.Platform ?? string.Empty,
                    UserId = c.UserId,
                    Username = c.Username,
                    CreatedBy = c.CreatedBy,
                    CreatedTime = c.CreatedTime,
                    UpdatedBy = c.UpdatedBy,
                    UpdatedTime = c.UpdatedTime
                }).ToDataSourceResultAsync(request, cancellationToken);

            return new SuccessResult<Result>(new Result(data));
        }
    }
}