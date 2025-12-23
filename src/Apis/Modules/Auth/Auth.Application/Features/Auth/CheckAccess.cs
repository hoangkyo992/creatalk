namespace Auth.Application.Features.Auth;

public class CheckAccess
{
    public record Request : IRequest<ApiResult<Result>>
    {
        public List<Feature> Features { get; init; }

        public record Feature(string Name, string Action)
        {
        }
    }

    public record Result
    {
        public bool IsAccessable { get; set; }
    }

    public class Validator : AbstractValidator<Request>
    {
        public Validator()
        {
        }
    }

    public class Handler(IAppContext appContext,
        ICurrentUser currentUser) : IRequestHandler<Request, ApiResult<Result>>
    {
        public async Task<ApiResult<Result>> Handle(Request request, CancellationToken cancellationToken)
        {
            if (request.Features.Count == 0)
            {
                return new SuccessResult<Result>(new Result
                {
                    IsAccessable = true
                });
            }

            if (currentUser.IsSystemUser)
            {
                return new SuccessResult<Result>(new Result
                {
                    IsAccessable = true
                });
            }

            var features = await appContext.UserRoles
                .Where(c => c.UserId == currentUser.Id)
                .SelectMany(c => c.Role.Features)
                .GroupBy(c => c.Feature)
                .Select(c => new
                {
                    Name = c.Key,
                    Actions = c.Select(cc => cc.Action)
                }).ToListAsync(cancellationToken);

            if (features.Count == 0)
                return new SuccessResult<Result>(new Result
                {
                    IsAccessable = false
                });

            var accessable = request.Features.Exists(req => features.Find(f => f.Name.Equals(req.Name, StringComparison.OrdinalIgnoreCase))?.Actions?.Any(a => a.ToUpper() == req.Action.ToUpper()) == true);

            return new SuccessResult<Result>(new Result
            {
                IsAccessable = accessable
            });
        }
    }
}