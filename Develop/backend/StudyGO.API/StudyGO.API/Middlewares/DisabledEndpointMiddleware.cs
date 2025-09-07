using StudyGO.API.CustomAttributes;

namespace StudyGO.API.Middlewares;

public class DisabledEndpointMiddleware
{
    private readonly RequestDelegate _next;

    public DisabledEndpointMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        var endpoint = context.GetEndpoint();

        var metadata = endpoint?.Metadata.GetMetadata<DisabledAttribute>();
        
        if (endpoint != null && metadata != null)
        {
            context.Response.StatusCode = StatusCodes.Status503ServiceUnavailable;
            await context.Response.WriteAsync(metadata.Description ?? "Этот ресурс временно недоступен");
            return;
        }

        await _next(context);
    }
}