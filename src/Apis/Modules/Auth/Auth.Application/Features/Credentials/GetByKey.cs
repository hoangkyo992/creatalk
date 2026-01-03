using Auth.Application.Features.Credentials.Dtos;

namespace Auth.Application.Features.Credentials;

public class GetByKey
{
    public record Request : IRequest<ApiResult<CredentialResDto?>>
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

    public class Handler(IAppContext context, IMapper mapper) : IRequestHandler<Request, ApiResult<CredentialResDto?>>
    {
        public async Task<ApiResult<CredentialResDto?>> Handle(Request request, CancellationToken cancellationToken)
        {
            var item = await context.Credentials
                .Where(c => c.Key.ToLower() == request.Key.ToLower())
                .AsNoTracking()
                .FirstOrDefaultAsync(cancellationToken);

            var setting = item == null ? null : mapper.Map<CredentialResDto>(item);

            return new SuccessResult<CredentialResDto?>(setting);
        }
    }
}