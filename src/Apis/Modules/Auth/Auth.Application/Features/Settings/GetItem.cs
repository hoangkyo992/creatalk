using Auth.Application.Features.Settings.Dtos;

namespace Auth.Application.Features.Settings;

public class GetItem
{
    public record Request : IRequest<ApiResult<SettingResDto>>
    {
        [JsonConverter(typeof(ZCodeJsonConverter))]
        public long Id { get; init; }
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

    public class Handler(IAppContext context, IMapper mapper) : IRequestHandler<Request, ApiResult<SettingResDto>>
    {
        public async Task<ApiResult<SettingResDto>> Handle(Request request, CancellationToken cancellationToken)
        {
            var item = await context.Settings
                .Where(c => c.Id == request.Id)
                .AsNoTracking()
                .FirstOrDefaultAsync(cancellationToken);
            if (item == null)
                return new FailResult<SettingResDto>(ErrorMessages.SETTING_NOT_FOUND, HttpStatusCode.NotFound);

            return new SuccessResult<SettingResDto>(mapper.Map<SettingResDto>(item));
        }
    }
}