using Auth.Application.Features.Auth;

namespace Auth.Application.Features.Roles;

public class GetFeatures
{
    public record Request : IRequest<ApiResult<Result>>
    {
        [JsonConverter(typeof(ZCodeJsonConverter))]
        public long Id { get; init; }
    }

    public record Result
    {
        public List<Feature> Features { get; set; }

        public List<AppFeature> AppFeatures { get; set; }

        public class Feature
        {
            public string Name { get; init; }
            public string Action { get; init; }
        }
    }

    public class Validator : AbstractValidator<Request>
    {
        public Validator()
        {
        }
    }

    public class Handler(IAppContext context, IMediator mediator, IFeatureManager featureManager) : IRequestHandler<Request, ApiResult<Result>>
    {
        public async Task<ApiResult<Result>> Handle(Request request, CancellationToken cancellationToken)
        {
            var roleFeatures = await context.RoleFeatures
                .Where(c => c.RoleId == request.Id)
                .Select(c => new Result.Feature
                {
                    Name = c.Feature,
                    Action = c.Action
                }).ToListAsync(cancellationToken);

            var accesses = (await mediator.Send(new GetAccesses.Request(), cancellationToken)).Result.Features;
            var appFeatures = featureManager.GetFeatures();
            return new SuccessResult<Result>(new Result
            {
                Features = roleFeatures,
                AppFeatures = appFeatures.Select(feature =>
                {
                    var access = accesses.Find(x => x.Name.Equals(feature.Name, StringComparison.OrdinalIgnoreCase));
                    if (access == null)
                        return null;
                    feature.Actions = feature.Actions.Where(x => access.Actions.Contains(x.Name, StringComparer.OrdinalIgnoreCase)).ToList();
                    return feature;
                })
                .Where(c => c != null)
                .Select(c => c!)
                .ToList()
            });
        }
    }
}