using System.Text.Json.Serialization;
using BuildingBlocks.Observability;
using Microsoft.AspNetCore.Routing.Constraints;

namespace Api.Users.Infrastructure.Http;

public static class Setup
{
    public static IServiceCollection AddHttp(this IServiceCollection services)
    {
        services
            .AddProblemDetails(o => o.CustomizeProblemDetails = context =>
            {
                context.ProblemDetails.Instance = context.HttpContext.GetRequestEndpoint();
                context.ProblemDetails.Extensions["traceId"] = context.HttpContext.TraceIdentifier;
            })
            .AddExceptionHandler<GlobalExceptionHandler>()
            .AddEndpointsApiExplorer()
            .AddSwaggerGen();

        services
            .Configure<RouteOptions>(options
                => options.ConstraintMap["regex"] = typeof(RegexInlineRouteConstraint))
            .ConfigureHttpJsonOptions(options =>
                options.SerializerOptions.Converters.Add(new JsonStringEnumConverter()));

        return services;
    }

    public static IApplicationBuilder UseHttp(this IApplicationBuilder app)
    {
        app.UseSwagger()
           .UseSwaggerUI();

        app.UseExceptionHandler();

        app.UseRouting();

        ((IEndpointRouteBuilder)app).MapUsersEndpoints();

        return app;
    }
}
