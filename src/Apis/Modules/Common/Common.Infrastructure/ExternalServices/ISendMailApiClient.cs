using Refit;

namespace Common.Infrastructure.ExternalServices;

[Headers("Authorization: Bearer", "Content-Type: application/json")]
public interface ISendMailApiClient
{
    [Post("/v3/mail/send")]
    Task SendEmailAsync(object content, CancellationToken cancellationToken = default);
}