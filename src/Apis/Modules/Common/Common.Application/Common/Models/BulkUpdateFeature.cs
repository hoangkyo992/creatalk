using MediatR;

namespace Common.Application.Common.Models;

public class BulkUpdateFeature
{
    public record Command : BulkUpdateCommand, IRequest<ApiResult<BulkUpdateResultDto>>
    {
    }

    public class Validator : BulkUpdateValidator<Command>
    {
    }
}