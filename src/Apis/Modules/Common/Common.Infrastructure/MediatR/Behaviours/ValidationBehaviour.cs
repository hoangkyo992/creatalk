using FluentValidation;

namespace Common.Infrastructure.MediatR.Behaviours;

public class ValidationBehavior<TRequest, TResponse>(IEnumerable<IValidator<TRequest>> validators) : IPipelineBehavior<TRequest, TResponse>
     where TRequest : notnull
{
    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
    {
        if (validators.Any())
        {
            var context = new ValidationContext<TRequest>(request);

            var validationResults = await Task.WhenAll(
                validators.Select(v =>
                    v.ValidateAsync(context, cancellationToken)));

            var failures = validationResults
                .Where(r => r.Errors.Count != 0)
                .SelectMany(r => r.Errors)
                .GroupBy(c => c.PropertyName)
                .Select(c => new
                {
                    Field = c.Key,
                    Errors = c.Select(cc => cc.ErrorCode)
                });

            if (failures.Any())
                throw ExceptionFactory.Create<ArgumentException>(CommonErrorMessages.MODEL_STATE_INVALID, "errors", failures);
        }
        return await next();
    }
}