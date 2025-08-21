using StudyGO.API.Middlewares;

namespace StudyGO.API.Extensions;

public static class DisabledEndpointMiddlewareExtensions
{
    public static IApplicationBuilder UseDisabledEndpoints(this IApplicationBuilder builder)
    {
        return builder.UseMiddleware<DisabledEndpointMiddleware>();
    }
}