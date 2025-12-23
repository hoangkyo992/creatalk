using System.Net;
using HungHd.Shared.Validations;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.Extensions.DependencyInjection;

namespace Common.Api.Attributes;

/// <summary>
/// Intriduces Model state auto validation
/// </summary>
/// <seealso cref="ActionFilterAttribute" />
public class ValidateModelAttribute : ActionFilterAttribute
{
    /// <summary>
    /// Validates Model automaticaly
    /// </summary>
    /// <param name="context"></param>
    /// <inheritdoc />
    public override void OnActionExecuting(ActionExecutingContext context)
    {
        var request = context.HttpContext.Request;
        // Check XSS in URL
        if (!string.IsNullOrWhiteSpace(request.Path)
            && CrossSiteScriptingValidation.IsDangerousString(request.Path, out string label))
        {
            throw ExceptionFactory.Create<CrossSiteScriptingException>(CommonErrorMessages.URL_XSS_ATTACK_DETECTED, "label", label);
        }

        // Check XSS in query string
        if (!string.IsNullOrWhiteSpace(request.QueryString.Value))
        {
            var queryString = WebUtility.UrlDecode(request.QueryString.Value);
            if (CrossSiteScriptingValidation.IsDangerousString(queryString, out label))
            {
                throw ExceptionFactory.Create<CrossSiteScriptingException>(CommonErrorMessages.QUERY_STRING_XSS_ATTACK_DETECTED, "label", label);
            }
        }

        ModelStateDictionary modelState = context.ModelState;
        if (modelState.IsValid)
        {
            Dictionary<string, object> errors = [];
            foreach (var item in modelState.Keys)
            {
                if (modelState.TryGetValue(item, out var entry)
                    && !string.IsNullOrWhiteSpace(entry.AttemptedValue)
                        && CrossSiteScriptingValidation.IsDangerousString(entry.AttemptedValue, out label))
                {
                    errors.Add(item, label);
                }
            }
            if (errors.Count > 0)
                throw ExceptionFactory.Create<CrossSiteScriptingException>(CommonErrorMessages.MODEL_STATE_XSS_ATTACK_DETECTED, errors);

            return;
        }
        var applicationOptions = request.HttpContext.RequestServices.GetRequiredService<IOptions<ApplicationOptions>>();
        if (applicationOptions.Value.UseMvcModelValidation)
        {
            throw ExceptionFactory.Create<ArgumentException>(CommonErrorMessages.MODEL_STATE_INVALID, "errors", ConvertModelStateErrors(modelState));
        }
    }

    private static List<(string Field, List<string> Errors)> ConvertModelStateErrors(ModelStateDictionary modelState)
    {
        List<(string Field, List<string> Errors)> errors = [];
        foreach (var item in modelState.Keys)
        {
            if (modelState.TryGetValue(item, out var entry)
                    && entry.ValidationState != ModelValidationState.Valid)
            {
                errors.Add((item, entry.Errors.Select(c => c.ErrorMessage).ToList()));
            }
        }
        return errors;
    }
}