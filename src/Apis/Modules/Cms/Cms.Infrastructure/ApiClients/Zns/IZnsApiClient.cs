using Refit;

namespace Cms.Infrastructure.ApiClients.Zns;

public interface IZnsApiClient
{
    [Post("/{path}")]
    [Headers("Content-Type: application/json")]
    Task<HttpResponseMessage> SendMessageAsync([AliasAs("path")] string path, [Body] SendMessageReqDto payload, CancellationToken cancellationToken = default);
}