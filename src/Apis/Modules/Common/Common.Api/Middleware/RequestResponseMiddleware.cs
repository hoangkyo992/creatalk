using System.Diagnostics;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Primitives;
using Serilog.Context;

namespace Common.Api.Middleware;

public class RequestResponseMiddleware(RequestDelegate next)
{
    private readonly RequestDelegate _next = next;

    public async Task Invoke(HttpContext context, ILogger<RequestResponseMiddleware> logger)
    {
        context.Request.Headers.TryGetValue(DefaultRequestHeaders.TraceIdentifierName, out StringValues traceId);
        using (LogContext.PushProperty(DefaultRequestHeaders.TraceCorrelationName, traceId))
        {
            var watch = Stopwatch.StartNew();
            context.Response.Headers.Append(DefaultRequestHeaders.TraceIdentifierName, context.TraceIdentifier);
            context.Items[DefaultRequestHeaders.TraceCorrelationName] = traceId;
            await _next.Invoke(context);
            watch.Stop();

            var processingTime = watch.ElapsedMilliseconds;
            using (LogContext.PushProperty(DefaultRequestHeaders.ResponseTime, processingTime))
            {
                logger.LogInformation("[{Method}]{Path} response with status code {StatusCode}, process time: {ProcessingTime}ms", context.Request.Method, context.Request.Path, context.Response.StatusCode, processingTime);
            }
        }
    }
}