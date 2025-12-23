namespace Auth.Application.Features.Users;

public class ListItem
{
    public record Request : DataSourceRequest, IRequest<ApiResult<Result>>
    {
    }

    public record Result : DataSourceResult<Result.Item>
    {
        public class Item : BaseDto
        {
            public string Username { get; init; }
            public string DisplayName { get; init; }
            public string Email { get; init; }
            public string? Phone { get; init; }
            public UserStatus StatusId { get; init; }
            public string StatusCode => EnumHelper<UserStatus>.GetLocalizedKey(StatusId);
            public DateTime? LastSignedIn { get; set; }
            public IEnumerable<string> Roles { get; init; } = [];
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
            var data = await context.Users
                .Where(c => c.Username != currentUser.Username)
                .SelectWithDefaultOrder(c => new Result.Item
                {
                    Id = c.Id,
                    Username = c.Username,
                    DisplayName = c.DisplayName,
                    Email = c.Email,
                    Phone = c.Phone,
                    StatusId = c.StatusId,
                    Roles = c.Roles.Select(cc => cc.Role.Name),
                    CreatedBy = c.CreatedBy,
                    CreatedTime = c.CreatedTime,
                    UpdatedBy = c.UpdatedBy,
                    UpdatedTime = c.UpdatedTime
                }).ToDataSourceResultAsync(request, cancellationToken);
            var userIds = data.Data.Select(c => c.Id).ToArray();
            var sessions = await context.UserSessions
                .Where(c => userIds.Contains(c.UserId))
                .GroupBy(c => c.UserId)
                .Select(c => new
                {
                    LoginId = c.Key,
                    StartTime = c.Select(cc => cc.StartTime).OrderByDescending(cc => cc).FirstOrDefault()
                }).ToListAsync(cancellationToken);

            foreach (var item in data.Data)
            {
                var session = sessions.Find(c => c.LoginId == item.Id);
                if (session != null)
                {
                    item.LastSignedIn = session.StartTime;
                }
            }
            return new SuccessResult<Result>(new Result(data));
        }
    }
}