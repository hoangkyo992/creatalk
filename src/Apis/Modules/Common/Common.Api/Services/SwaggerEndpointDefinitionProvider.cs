using Asp.Versioning.ApiExplorer;
using Swashbuckle.AspNetCore.Swagger;

namespace Common.Api.Services;

public class SwaggerEndpointDefinitionProvider(ISwaggerProvider swaggerProvider,
    IApiVersionDescriptionProvider apiVersionDescriptionProvider) : IEndpointDefinitionProvider
{
    private readonly ISwaggerProvider _swaggerProvider = swaggerProvider;
    private readonly IApiVersionDescriptionProvider _apiVersionDescriptionProvider = apiVersionDescriptionProvider;

    public IEnumerable<EndpointDefinition> GetEndpoints(string? host = null, string? basePath = null)
    {
        var endpointDefinitions = new List<EndpointDefinition>();
        foreach (var description in _apiVersionDescriptionProvider.ApiVersionDescriptions)
        {
            var apiDocument = _swaggerProvider.GetSwagger(description.GroupName, host, basePath);
            if (apiDocument != null)
            {
                var endpoints = apiDocument.Paths
                .SelectMany(c => c.Value.Operations
                    .Select(cc => new EndpointDefinition
                    {
                        Path = c.Key,
                        Version = description.ApiVersion.ToString(),
                        Method = cc.Key.ToString(),
                        Prefix = ApiConstants.ApiPrefix,
                        IsDeprecated = cc.Value.Deprecated,
                        Description = cc.Value.Description,
                        Summary = cc.Value.Summary,
                        Controller = cc.Value.OperationId.Split('_').FirstOrDefault() ?? ValueConstants.NaN,
                        ActionName = cc.Value.OperationId.Split('_').Skip(1).FirstOrDefault() ?? ValueConstants.NaN
                    }))
                .OrderBy(c => c.Controller)
                .ThenBy(c => c.ActionName)
                .ToList();
                endpointDefinitions.AddRange(endpoints);
            }
        }
        return endpointDefinitions;
    }
}