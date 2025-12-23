using Auth.Application.Features.Settings.Dtos;

namespace Auth.Application.Features.Settings;

public class GetByKey
{
    public record Request : IRequest<ApiResult<SettingResDto?>>
    {
        public string Key { get; init; }
    }

    public class Validator : AbstractValidator<Request>
    {
        public Validator()
        {
            RuleFor(v => v.Key)
                .NotNull()
                .NotEmpty();
        }
    }

    public class Handler(IAppContext context, IMapper mapper) : IRequestHandler<Request, ApiResult<SettingResDto?>>
    {
        public async Task<ApiResult<SettingResDto?>> Handle(Request request, CancellationToken cancellationToken)
        {
            var item = await context.Settings
                .Where(c => c.Key.ToLower() == request.Key.ToLower())
                .AsNoTracking()
                .FirstOrDefaultAsync(cancellationToken);

            var setting = item == null ? null : mapper.Map<SettingResDto>(item);

            return new SuccessResult<SettingResDto?>(setting);
        }
    }
}