namespace SortFlow.Api.Middleware;

public class CorrelationIdMiddleware
{
    private const string Header = "X-Correlation-Id";
    private readonly RequestDelegate _next;

    public CorrelationIdMiddleware(RequestDelegate next) => _next = next;

    public async Task InvokeAsync(HttpContext context)
    {
        var id = context.Request.Headers[Header].FirstOrDefault() ?? Guid.NewGuid().ToString("N")[..16];
        context.Response.Headers[Header] = id;
        context.TraceIdentifier = id;
        await _next(context);
    }
}
