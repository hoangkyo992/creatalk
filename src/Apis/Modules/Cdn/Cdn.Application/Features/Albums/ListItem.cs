using HungHd.LinqExtensions;

namespace Cdn.Application.Features.Albums;

public class ListItem
{
    public record Request : DataSourceRequest, IRequest<ApiResult<Result>>
    {
    }

    public record Result : DataSourceResult<Result.ItemDto>
    {
        public class ItemDto : BaseDto
        {
            public string Name { get; init; }
            public string Description { get; init; }
            public int NumberOfItems { get; init; }
        }

        public Result(DataSourceResult<ItemDto> result) : base(result)
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
            var data = await appContext.Albums
                .SelectWithDefaultOrder(c => new Result.ItemDto
                {
                    Id = c.Id,
                    Name = c.Name,
                    Description = c.Description,
                    NumberOfItems = c.Files.Count(c => !c.File.IsDeleted),
                    CreatedBy = c.CreatedBy,
                    CreatedTime = c.CreatedTime,
                    UpdatedBy = c.UpdatedBy,
                    UpdatedTime = c.UpdatedTime
                }).ToDataSourceResultAsync(request, cancellationToken);

            return new SuccessResult<Result>(new Result(data));
        }
    }
}