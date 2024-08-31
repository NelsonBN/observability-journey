using System.Diagnostics;
using Microsoft.AspNetCore.Http;

namespace BuildingBlocks.Observability;

public static class HttpContextExtensions
{
    public static string GetTraceId(this HttpContext httpContext)
    {
        var traceId = Activity.Current?.TraceId.ToString();
        if(!string.IsNullOrWhiteSpace(traceId))
        {
            return traceId;
        }

        traceId = httpContext?.TraceIdentifier;
        if(string.IsNullOrWhiteSpace(traceId))
        {
            traceId = $"guid:{Guid.NewGuid()}";
        }

        return traceId;
    }

    public static string? GetRequestEndpoint(this HttpContext httpContext)
    {
        if(httpContext?.Request is null)
        {
            return null;
        }

        return $"{httpContext.Request.Method}: {httpContext.Request.Path.Value}";
    }
}
