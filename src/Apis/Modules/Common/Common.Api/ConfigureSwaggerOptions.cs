using Asp.Versioning.ApiExplorer;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace Common.Api;

public class ConfigureSwaggerOptions(IApiVersionDescriptionProvider provider, IOptions<ApplicationOptions> options) : IConfigureNamedOptions<SwaggerGenOptions>
{
    private readonly IApiVersionDescriptionProvider _provider = provider;
    private readonly ApplicationOptions _applicationOptions = options.Value;

    public void Configure(SwaggerGenOptions options)
    {
        // Add swagger document for every API version discovered
        foreach (var description in _provider.ApiVersionDescriptions)
        {
            options.SwaggerDoc(
                description.GroupName,
                CreateVersionInfo(description));
        }
        options.EnableAnnotations();
        var fileName = $"{AppDomain.CurrentDomain.FriendlyName}.xml";
        var filePath = Path.Combine(AppContext.BaseDirectory, fileName);
        if (File.Exists(filePath))
        {
            options.IncludeXmlComments(filePath, true);
        }
        options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
        {
            Type = SecuritySchemeType.Http,
            BearerFormat = "Custom",
            In = ParameterLocation.Header,
            Scheme = "Bearer"
        });
        options.OperationFilter<DefaultRequestHeader>();
        options.CustomSchemaIds(type => type.FullName?.Replace("+", ".") ?? type.FullName);
        options.CustomOperationIds(apiDesc =>
        {
            apiDesc.ActionDescriptor.RouteValues.TryGetValue("action", out var action);
            return $"{apiDesc.ActionDescriptor.RouteValues["controller"]}_{(action ?? "__")}_{apiDesc.HttpMethod}";
        });
    }

    public void Configure(string? name, SwaggerGenOptions options)
    {
        Configure(options);
    }

    private OpenApiInfo CreateVersionInfo(ApiVersionDescription description)
    {
        var info = new OpenApiInfo()
        {
            Title = _applicationOptions.ServiceName,
            Version = description.ApiVersion.ToString()
        };

        if (description.IsDeprecated)
        {
            info.Description += " This API version has been deprecated.";
        }

        return info;
    }
}

public class DefaultRequestHeader : IOperationFilter
{
    public void Apply(OpenApiOperation operation, OperationFilterContext context)
    {
        operation.Security ??= [];

        var scheme = new OpenApiSecurityScheme
        {
            Reference = new OpenApiReference
            {
                Id = "Bearer",
                Type = ReferenceType.SecurityScheme
            }
        };
        operation.Security.Add(new OpenApiSecurityRequirement
        {
            [scheme] = []
        });
    }
}