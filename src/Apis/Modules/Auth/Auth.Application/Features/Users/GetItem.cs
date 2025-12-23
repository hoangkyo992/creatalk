namespace Auth.Application.Features.Users;

public class GetItem
{
    public record Request : IRequest<ApiResult<Result>>
    {
        [JsonConverter(typeof(ZCodeJsonConverter))]
        public long Id { get; init; }
    }

    public class Result : BaseDto
    {
        public string Username { get; init; }
        public string DisplayName { get; init; }
        public string Email { get; init; }
        public string? Phone { get; init; }
        public UserStatus StatusId { get; init; }
        public string StatusCode => EnumHelper<UserStatus>.GetLocalizedKey(StatusId);

        [JsonConverter(typeof(ZCodeCollectionJsonConverter<IEnumerable<long>>))]
        public IEnumerable<long> RoleIds { get; init; } = [];
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

    public class Handler(IAppContext context) : IRequestHandler<Request, ApiResult<Result>>
    {
        public async Task<ApiResult<Result>> Handle(Request request, CancellationToken cancellationToken)
        {
            var item = await context.Users
                .Where(c => c.Id == request.Id)
                .Select(c => new Result
                {
                    Id = c.Id,
                    Username = c.Username,
                    DisplayName = c.DisplayName,
                    Email = c.Email,
                    Phone = c.Phone,
                    StatusId = c.StatusId,
                    RoleIds = c.Roles.Select(cc => cc.RoleId).Distinct(),
                    CreatedBy = c.CreatedBy,
                    CreatedTime = c.CreatedTime,
                    UpdatedBy = c.UpdatedBy,
                    UpdatedTime = c.UpdatedTime
                }).FirstOrDefaultAsync(cancellationToken);
            if (item == null)
                return new FailResult<Result>(ErrorMessages.USER_NOT_FOUND, HttpStatusCode.NotFound);

            return new SuccessResult<Result>(item);
        }
    }
}