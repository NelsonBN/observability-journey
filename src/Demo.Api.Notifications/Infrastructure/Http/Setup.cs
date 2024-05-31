using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Routing.Constraints;

namespace Api.Notifications.Infrastructure.Http;

public static class Setup
{
    public static IServiceCollection AddHttp(this IServiceCollection services)
    {
        services
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

        ((IEndpointRouteBuilder)app).MapNotificationsEndpoints();

        return app;
    }
}
