namespace Auth.Application.Features.Settings;

public class ListItem
{
    public record Request : DataSourceRequest, IRequest<ApiResult<Result>>
    {
    }

    public record Result : DataSourceResult<Result.Item>
    {
        public class Item : BaseDto
        {
            public string Key { get; init; }
            public string Value { get; init; }
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
            var data = await context.Settings
                .SelectWithDefaultOrder(c => new Result.Item
                {
                    Id = c.Id,
                    Key = c.Key,
                    Value = c.Value,
                    CreatedBy = c.CreatedBy,
                    CreatedTime = c.CreatedTime,
                    UpdatedBy = c.UpdatedBy,
                    UpdatedTime = c.UpdatedTime
                }).ToDataSourceResultAsync(request, cancellationToken);

            return new SuccessResult<Result>(new Result(data));
        }
    }
}