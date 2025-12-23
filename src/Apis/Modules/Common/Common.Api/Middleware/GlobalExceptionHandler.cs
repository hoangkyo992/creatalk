using System.Collections;
using System.Net;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Http;

namespace Common.Api.Middleware;

public sealed class GlobalExceptionHandler(ILogger<GlobalExceptionHandler> logger,
    IOptions<ApplicationOptions> applicationOptions) : IExceptionHandler
{
    private readonly ILogger<GlobalExceptionHandler> _logger = logger;
    private readonly ApplicationOptions _applicationOptions = applicationOptions.Value;

    public async ValueTask<bool> TryHandleAsync(
        HttpContext httpContext,
        Exception exception,
        CancellationToken cancellationToken)
    {
        await HandleException(httpContext, exception, exception.GetHttpStatusCode(), cancellationToken);
        return true;
    }

    private async Task HandleException(HttpContext context,
        Exception ex,
        HttpStatusCode statusCode,
        CancellationToken cancellationToken)
    {
        _logger.LogError(ex, "An error occurred: {Message}", ex.Message);

        if (_applicationOptions.UseSentry)
        {
            SentrySdk.ConfigureScope(scope =>
            {
                context.Request.Headers.TryGetValue("X-Forwarded-For", out var clientIpAddress);
                var user = new SentryUser
                {
                    IpAddress = $"{clientIpAddress}"
                };
                if (context.User?.Identity?.IsAuthenticated == true)
                {
                    user.Id = context.User.FindFirst(AppClaimTypes.Username)?.Value ?? string.Empty;
                }
                scope.User = user;
            });
            SentrySdk.CaptureException(ex);
        }

        context.Response.StatusCode = (int)statusCode;
        var response = new ErrorResponseModel((int)statusCode, ex);
        if (_applicationOptions.IncludeInnerException)
            response.Exception = ex.InnerException?.ToString();
        await context.Response.WriteAsJsonAsync(response, cancellationToken);
    }

    private sealed class ErrorResponseModel(int statusCode, Exception ex)
    {
        public bool IsSuccess { get; private set; } = false;
        public int StatusCode { get; set; } = statusCode;
        public string FailureReason { get; set; } = ex.Message;
        public IDictionary Data { get; private set; } = ex.Data;

        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public string? Exception { get; set; }
    }
}