namespace Common.Api.Services;

public interface IEndpointDefinitionProvider
{
    IEnumerable<EndpointDefinition> GetEndpoints(string? host = null, string? basePath = null);
}