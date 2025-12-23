namespace Auth.Application.Features.Roles;

public class GetItem
{
    public record Request : IRequest<ApiResult<Result>>
    {
        [JsonConverter(typeof(ZCodeJsonConverter))]
        public long Id { get; init; }
    }

    public class Result : BaseDto
    {
        public string Name { get; init; }
        public string Description { get; init; }
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
            var item = await context.Roles
                .Where(c => c.Id == request.Id && c.Id != (int)SystemRole.SuperAdmin)
                .Select(c => new Result
                {
                    Id = c.Id,
                    Name = c.Name,
                    Description = c.Description,
                    CreatedBy = c.CreatedBy,
                    CreatedTime = c.CreatedTime,
                    UpdatedBy = c.UpdatedBy,
                    UpdatedTime = c.UpdatedTime
                }).FirstOrDefaultAsync(cancellationToken);
            if (item == null)
                return new FailResult<Result>(ErrorMessages.ROLE_NOT_FOUND, HttpStatusCode.NotFound);

            return new SuccessResult<Result>(item);
        }
    }
}