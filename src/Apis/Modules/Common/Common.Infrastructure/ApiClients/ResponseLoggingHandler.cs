namespace Common.Infrastructure.ApiClients;

public sealed class ResponseLoggingHandler(ILogger<ResponseLoggingHandler> logger) : DelegatingHandler
{
    protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        var response = await base.SendAsync(request, cancellationToken);
        if (!response.IsSuccessStatusCode)
        {
            var content = await response.Content.ReadAsStringAsync(cancellationToken);
            logger.LogError("An error has occurred when calling api [{HttpMethod}]{Request}.\r\n => StatusCode: {StatusCode}.\r\n Response content: {Content}",
                request.Method,
                request.RequestUri,
                response.StatusCode,
                content);
        }
        return response;
    }
}