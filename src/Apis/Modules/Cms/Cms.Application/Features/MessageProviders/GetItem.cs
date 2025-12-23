namespace Cms.Application.Features.MessageProviders;

public class GetItem
{
    public record Request : IRequest<ApiResult<Result>>
    {
        [JsonConverter(typeof(ZCodeJsonConverter))]
        public long Id { get; init; }
    }

    public class Result : BaseDto
    {
        public string Code { get; init; }
        public string Name { get; init; }
        public string Settings { get; init; }
        public bool IsDisabled { get; init; }
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
            var item = await appContext.MessageProviders
                .Where(c => c.Id == request.Id)
                .Select(c => new Result
                {
                    Id = c.Id,
                    Code = c.Code,
                    Name = c.Name,
                    Settings = c.Settings,
                    IsDisabled = c.IsDisabled,
                    CreatedBy = c.CreatedBy,
                    CreatedTime = c.CreatedTime,
                    UpdatedBy = c.UpdatedBy,
                    UpdatedTime = c.UpdatedTime
                }).FirstOrDefaultAsync(cancellationToken);

            return item == null
                ? new FailResult<Result>(ErrorMessages.MESSAGE_PROVIDER_NOT_FOUND, HttpStatusCode.NotFound)
                : new SuccessResult<Result>(item);
        }
    }
}