using Refit;

namespace Cms.Infrastructure.ApiClients.Zns;

public interface IZnsApiClient
{
    [Post("/message/template")]
    Task<HttpResponseMessage> SendMessageAsync(SendMessageReqDto payload, CancellationToken cancellationToken = default);
}