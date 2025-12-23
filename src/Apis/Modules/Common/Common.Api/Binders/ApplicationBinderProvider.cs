using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.ModelBinding.Binders;
using Microsoft.AspNetCore.Mvc.ModelBinding.Metadata;

namespace Common.Api.Binders;

public class ApplicationBinderProvider : IModelBinderProvider
{
    public IModelBinder? GetBinder(ModelBinderProviderContext context)
    {
        ArgumentNullException.ThrowIfNull(context);
        return context.Metadata.ModelType == typeof(DateTime) || context.Metadata.ModelType == typeof(Nullable<DateTime>)
            ? new BinderTypeModelBinder(typeof(DateTimeBinder))
            : context.Metadata.ModelType == typeof(Int64) || context.Metadata.ModelType == typeof(Nullable<Int64>)
            ? CreateInt64CustomBinder(context)
            : null;
    }

    private static BinderTypeModelBinder? CreateInt64CustomBinder(ModelBinderProviderContext context)
    {
        if (context.Metadata is DefaultModelMetadata modelMetadata)
        {
            if (modelMetadata.Attributes.PropertyAttributes != null)
            {
                var jsonConverterAttribute = modelMetadata
                .Attributes
                .PropertyAttributes
                .Where(c => c.GetType() == typeof(JsonConverterAttribute))
                .Select(c => c as JsonConverterAttribute)
                .FirstOrDefault();
                if (jsonConverterAttribute != null && jsonConverterAttribute.ConverterType == typeof(ZCodeJsonConverter))
                {
                    return new BinderTypeModelBinder(typeof(ZCodeToLongBinder));
                }
            }

            if (modelMetadata.Attributes.ParameterAttributes != null)
            {
                var jsonConverterAttribute = modelMetadata
                .Attributes
                .ParameterAttributes
                .Where(c => c.GetType() == typeof(ZCodeToInt64Attribute))
                .Select(c => c as ZCodeToInt64Attribute)
                .FirstOrDefault();
                if (jsonConverterAttribute != null)
                {
                    return new BinderTypeModelBinder(typeof(ZCodeToLongBinder));
                }
            }
        }
        return null;
    }
}