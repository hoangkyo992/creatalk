namespace Auth.Application.Features.Roles;

public class ListItem
{
    public record Request : DataSourceRequest, IRequest<ApiResult<Result>>
    {
    }

    public record Result : DataSourceResult<Result.Item>
    {
        public class Item : BaseDto
        {
            public string Name { get; init; }
            public string Description { get; init; }
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

    public class Handler(IAppContext context) : IRequestHandler<Request, ApiResult<Result>>
    {
        public async Task<ApiResult<Result>> Handle(Request request, CancellationToken cancellationToken)
        {
            var data = await context.Roles
                .Where(c => c.Id != (int)SystemRole.SuperAdmin)
                .SelectWithDefaultOrder(c => new Result.Item
                {
                    Id = c.Id,
                    Name = c.Name,
                    Description = c.Description,
                    CreatedBy = c.CreatedBy,
                    CreatedTime = c.CreatedTime,
                    UpdatedBy = c.UpdatedBy,
                    UpdatedTime = c.UpdatedTime
                }).ToDataSourceResultAsync(request, cancellationToken);

            return new SuccessResult<Result>(new Result(data));
        }
    }
}