using FluentValidation;

namespace Common.Application.Common.Models;

public class BulkUpdateValidator<T> : AbstractValidator<T> where T : BulkUpdateCommand
{
    public BulkUpdateValidator()
    {
        RuleFor(v => v.Ids)
            .NotNull()
            .NotEmpty();
    }
}