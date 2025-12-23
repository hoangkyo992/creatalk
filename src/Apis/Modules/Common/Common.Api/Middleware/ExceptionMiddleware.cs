using System.Collections;
using System.Net;
using Microsoft.AspNetCore.Http;

namespace Common.Api.Middleware;

public class ExceptionMiddleware(RequestDelegate next,
    IOptions<ApplicationOptions> options,
    ILogger<ExceptionMiddleware> logger)
{
    private readonly RequestDelegate _next = next;
    private readonly ILogger<ExceptionMiddleware> _logger = logger;
    private readonly ApplicationOptions _applicationOptions = options.Value;

    public async Task Invoke(HttpContext context)
    {
        try
        {
            await _next.Invoke(context);
        }
        catch (Exception ex)
        {
            await HandleException(context, ex, ex.GetHttpStatusCode());
        }
    }

    private async Task HandleException(HttpContext context, Exception ex, HttpStatusCode statusCode)
    {
        _logger.LogError(ex, "An error occurred: {Message}", ex.Message);
        context.Response.StatusCode = (int)statusCode;
        var response = new ErrorResponseModel((int)statusCode, ex);
        if (_applicationOptions.IncludeInnerException)
            response.Exception = ex.InnerException?.ToString();
        await context.Response.WriteAsJsonAsync(response);
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