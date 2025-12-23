using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace Common.Api.Binders;

/// <summary>
/// ZCode to long binder
/// </summary>
public class ZCodeToLongBinder : IModelBinder
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

        if (!ZCode.TryGetInt64(value, out var val))
        {
            throw ExceptionFactory.Create<ArgumentException>(CommonErrorMessages.PARAMETER_CONVERT_FAILED,
                new
                {
                    modelName,
                    type = typeof(Int64).Name,
                    value
                }.ToDictionaryEntries());
        }

        bindingContext.Result = ModelBindingResult.Success(val);
        return Task.CompletedTask;
    }
}