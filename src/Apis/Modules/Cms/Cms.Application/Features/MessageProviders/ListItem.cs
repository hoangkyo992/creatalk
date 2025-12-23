namespace Cms.Application.Features.MessageProviders;

public class ListItem
{
    public record Request : DataSourceRequest, IRequest<ApiResult<Result>>
    {
    }

    public record Result : DataSourceResult<Result.Item>
    {
        public class Item : BaseDto
        {
            public string Code { get; init; }
            public string Name { get; init; }
            public string Settings { get; init; }
            public bool IsDisabled { get; init; }
        }

        public Result(DataSourceResult<Item> result) : base(result)
        {
        }
    }

    public class Validator : AbstractValidator<Request>
    {
        public Validator()
        {
        }
    }

    public class Handler(IAppContext appContext) : IRequestHandler<Request, ApiResult<Result>>
    {
        public async Task<ApiResult<Result>> Handle(Request request, CancellationToken cancellationToken)
        {
            var data = await appContext.MessageProviders
                .SelectWithDefaultOrder(c => new Result.Item
                {
                    Id = c.Id,
                    Code = c.Code,
                    Name = c.Name,
                    IsDisabled = c.IsDisabled,
                    Settings = c.Settings,
                    CreatedBy = c.CreatedBy,
                    CreatedTime = c.CreatedTime,
                    UpdatedBy = c.UpdatedBy,
                    UpdatedTime = c.UpdatedTime
                }).ToDataSourceResultAsync(request, cancellationToken);
            return new SuccessResult<Result>(new Result(data));
        }
    }
}