using Refit;

namespace Cms.Infrastructure.ApiClients.Zns;

public interface IZnsApiClient
{
    [Post("/message/template")]
    Task<SendMessageResDto> SendMessageAsync([Header("access_token")] string token, SendMessageReqDto payload, CancellationToken cancellationToken = default);
}