using Api.Notifications.Infrastructure.Database;
using Common;
using Common.Observability;
using HealthChecks.UI.Client;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Npgsql;
using OpenTelemetry;
using OpenTelemetry.Logs;
using OpenTelemetry.Metrics;
using OpenTelemetry.Trace;

namespace Api.Notifications.Infrastructure.Observability;

public static class Setup
{
    public static IServiceCollection AddObservability(this IServiceCollection services)
    {
        services
            .AddOpenTelemetry()
            .UseOtlpExporter()
            .ConfigureResource(TelemetryFactory.Configure)
            .WithTracing(options
                => options
                    .AddSource(AppDetails.Name)
                    .SetResourceBuilder(TelemetryFactory.CreateResource())
                    .AddAspNetCoreInstrumentation()
                    .AddHttpClientInstrumentation()
                    .AddNpgsql()
                    .AddEntityFrameworkCoreInstrumentation()
                    .AddRedisInstrumentation())
            .WithMetrics(options =>
                options
                    .SetResourceBuilder(TelemetryFactory.CreateResource())
                    .AddAspNetCoreInstrumentation()
                    .AddHttpClientInstrumentation()
                    .AddRuntimeInstrumentation()
                    .AddMeter(Diagnostic.Meter.Name)
                    .AddView(
                        "http.server.request.duration",
                        new ExplicitBucketHistogramConfiguration { Boundaries = [0, 0.005, 0.01, 0.025, 0.05, 0.075, 0.1, 0.25, 0.5, 0.75, 1, 2.5, 5, 7.5, 10] }));

        services._addHealth();

        return services;
    }

    private static IServiceCollection _addHealth(this IServiceCollection services)
    {
        services
            .AddHealthChecks()
            .AddDbContextCheck<DataContext>("EFCore", HealthStatus.Unhealthy)
            .AddRabbitMQ("RabbitMQ", HealthStatus.Unhealthy)
            .AddRedis(
                name: "Redis",
                connectionStringFactory: (sp) => sp.GetRequiredService<IConfiguration>().GetConnectionString("Redis")!,
                failureStatus: HealthStatus.Unhealthy);

        return services;
    }

    public static ILoggingBuilder AddObservability(this ILoggingBuilder logging)
        => logging
            .AddOpenTelemetry(options =>
            {
                options.IncludeScopes = true;
                options.IncludeFormattedMessage = true;
                options.ParseStateValues = true;

                options
                    .SetResourceBuilder(TelemetryFactory.CreateResource());
            });

    public static IApplicationBuilder AddObservability(this IApplicationBuilder app)
    {
        app.UseEndpoints(endpoints => endpoints.MapHealthChecks("/health", new HealthCheckOptions
        {
            ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse
        }));

        return app;
    }
}
