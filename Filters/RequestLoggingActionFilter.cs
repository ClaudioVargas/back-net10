using System.Diagnostics;
using Microsoft.AspNetCore.Mvc.Filters;

namespace back.Filters;

public sealed class RequestLoggingActionFilter : IAsyncActionFilter
{
    private readonly ILogger<RequestLoggingActionFilter> _logger;

    public RequestLoggingActionFilter(ILogger<RequestLoggingActionFilter> logger)
    {
        _logger = logger;
    }

    public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
    {
        var start = Stopwatch.StartNew();
        var requestId = context.HttpContext.TraceIdentifier;

        context.HttpContext.Response.OnStarting(() =>
        {
            context.HttpContext.Response.Headers["X-Request-Id"] = requestId;
            context.HttpContext.Response.Headers["X-Response-Time-ms"] = start.ElapsedMilliseconds.ToString();
            return Task.CompletedTask;
        });

        _logger.LogInformation(
            "Request {Method} {Path} started.",
            context.HttpContext.Request.Method,
            context.HttpContext.Request.Path);

        var executedContext = await next();

        _logger.LogInformation(
            "Request {Method} {Path} completed in {ElapsedMs} ms with status {StatusCode}.",
            context.HttpContext.Request.Method,
            context.HttpContext.Request.Path,
            start.ElapsedMilliseconds,
            executedContext?.HttpContext.Response.StatusCode ?? 200);
    }
}
