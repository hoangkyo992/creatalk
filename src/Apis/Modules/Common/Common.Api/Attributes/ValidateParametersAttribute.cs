using System.Reflection;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Mvc.Filters;

namespace Common.Api.Attributes;

/// <summary>
///
/// </summary>
public class ValidateParametersAttribute : ActionFilterAttribute
{
    private const string OptionalAttributeKey = "OptionalAttribute";

    public override void OnActionExecuting(ActionExecutingContext context)
    {
        if (context.ActionDescriptor is ControllerActionDescriptor descriptor)
        {
            var parameters = descriptor.MethodInfo.GetParameters();
            var nullChecks = CheckParameterRequired(context, parameters);
            if (nullChecks.Count > 0)
                throw ExceptionFactory.Create<ArgumentException>(CommonErrorMessages.PARAMETER_REQUIRED, "parameters", nullChecks.Join());
        }

        base.OnActionExecuting(context);
    }

    private static List<string> CheckParameterRequired(ActionExecutingContext context, IEnumerable<ParameterInfo> parameters)
    {
        List<string> nullParameters = [];
        foreach (var parameter in parameters)
        {
            if (parameter == null)
                continue;
            var required = !parameter.CustomAttributes.Any() ||
                !parameter.CustomAttributes.Any(item => item.AttributeType.ToString().Contains(OptionalAttributeKey));
            if (!required)
                continue;

            var valueGetted = context.ActionArguments.TryGetValue(parameter.Name ?? "", out object? value);
            if (!valueGetted || value == null)
            {
                nullParameters.Add(parameter.Name ?? "");
            }
        }
        return nullParameters;
    }
}