using System.Globalization;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace Common.Api.Binders;

/// <summary>
/// Date time binder
/// </summary>
public class DateTimeBinder : IModelBinder
{
    public Task BindModelAsync(ModelBindingContext bindingContext)
    {
        ArgumentNullException.ThrowIfNull(bindingContext);

        var modelName = bindingContext.ModelName;

        // Try to fetch the value of the argument by name
        var valueProviderResult = bindingContext.ValueProvider.GetValue(modelName);

        if (valueProviderResult == ValueProviderResult.None)
        {
            return Task.CompletedTask;
        }

        bindingContext.ModelState.SetModelValue(modelName, valueProviderResult);

        var value = valueProviderResult.FirstValue;

        // Check if the argument value is null or empty
        if (string.IsNullOrEmpty(value))
        {
            return Task.CompletedTask;
        }

        var culture = valueProviderResult.Culture;
        if (!DateTime.TryParse(value, culture, DateTimeStyles.AdjustToUniversal, out var date))
        {
            throw ExceptionFactory.Create<ArgumentException>(CommonErrorMessages.PARAMETER_CONVERT_FAILED,
                new
                {
                    modelName,
                    type = typeof(DateTime).Name,
                    value
                }.ToDictionaryEntries());
        }

        bindingContext.Result = ModelBindingResult.Success(DateTime.SpecifyKind(date, DateTimeKind.Utc));
        return Task.CompletedTask;
    }
}