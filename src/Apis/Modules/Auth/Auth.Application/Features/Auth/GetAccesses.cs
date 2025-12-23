namespace Auth.Application.Features.Auth;

public class GetAccesses
{
    public record Request : IRequest<ApiResult<Result>>
    {
    }

    public record Result
    {
        public List<Feature> Features { get; set; }
        public class Feature
        {
            public string Name { get; set; }
            public List<string> Actions { get; set; }
        }
    }

    public class Validator : AbstractValidator<Request>
    {
        public Validator()
        {
        }
    }

    public class Handler(IAppContext appContext,
        IFeatureManager featureManager,
        ICurrentUser currentUser) : IRequestHandler<Request, ApiResult<Result>>
    {
        public async Task<ApiResult<Result>> Handle(Request request, CancellationToken cancellationToken)
        {
            if (currentUser.IsSystemUser)
            {
                return new SuccessResult<Result>(new Result
                {
                    Features = [.. featureManager.GetFeatures().Select(c => new Result.Feature
                    {
                        Name = c.Name,
                        Actions = [.. c.Actions.Select(x => x.Name.ToUpper())]
                    })]
                });
            }

            var features = await appContext.UserRoles
                .Where(c => c.UserId == currentUser.Id)
                .SelectMany(c => c.Role.Features)
                .GroupBy(c => c.Feature)
                .Select(c => new Result.Feature
                {
                    Name = c.Key,
                    Actions = c.Select(cc => cc.Action).ToList()
                }).ToListAsync(cancellationToken);

            return new SuccessResult<Result>(new Result
            {
                Features = features
            });
        }
    }
}