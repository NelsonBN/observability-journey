using Api.Notifications.Infrastructure.Database;
using BuildingBlocks.Observability;
using HealthChecks.UI.Client;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Npgsql;
using OpenTelemetry;
using OpenTelemetry.Logs;
using OpenTelemetry.Metrics;
using OpenTelemetry.Trace;
using Quartz;

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
                    .AddSource(Telemetry.Source.Name)
                    .AddAspNetCoreInstrumentation(o => o.RecordException = true)
                    .AddHttpClientInstrumentation(o => o.RecordException = true)
                    .AddGrpcCoreInstrumentation()
                    .AddNpgsql()
                    .AddEntityFrameworkCoreInstrumentation()
                    .AddQuartzInstrumentation(o =>
                    {
                        o.RecordException = true;
                        o.Enrich = (activity, eventName, quartzJobDetails) =>
                        {
                            if(eventName == "OnStartActivity" && quartzJobDetails is IJobDetail onStartJobDetail)
                            {
                                if(onStartJobDetail.JobDataMap.TryGetValue("ScheduleCron", out var value))
                                {
                                    activity.SetTag("scheduler.cron", value);
                                }
                            }

                            else if(eventName == "OnStopActivity" && quartzJobDetails is IJobDetail onStopJobDetails)
                            {
                                if(onStopJobDetails.JobDataMap.TryGetValue("ScheduleNext", out var value))
                                {
                                    activity.SetTag("scheduler.next", value);
                                }
                            }
                        };
                    }))
            .WithMetrics(options =>
                options
                    .AddMeter(Telemetry.Meter.Name)
                    .AddAspNetCoreInstrumentation()
                    .AddHttpClientInstrumentation()
                    .AddRuntimeInstrumentation()
                    .AddView(
                        "http.server.request.duration",
                        new ExplicitBucketHistogramConfiguration { Boundaries = [0, 0.005, 0.01, 0.025, 0.05, 0.075, 0.1, 0.25, 0.5, 0.75, 1, 2.5, 5, 7.5, 10] }))
            .WithLogging(options =>
                options.SetResourceBuilder(TelemetryFactory.CreateResource()),
                configureOptions =>
                {
                    configureOptions.IncludeFormattedMessage = true;
                    configureOptions.IncludeScopes = true;
                    configureOptions.ParseStateValues = true;
                });

        services._addHealth();

        return services;
    }

    private static IServiceCollection _addHealth(this IServiceCollection services)
    {
        services
            .AddSingleton<StartupBackgroundService.HealthCheck>()
            .AddHostedService<StartupBackgroundService>()
            .AddHealthChecks()
            .AddDbContextCheck<DataContext>("EFCore", HealthStatus.Unhealthy)
            .AddRabbitMQ("RabbitMQ", HealthStatus.Unhealthy)
            .AddCheck<StartupBackgroundService.HealthCheck>(
                "Startup",
                tags: ["Startup"]);

        return services;
    }

    public static IApplicationBuilder AddObservability(this IApplicationBuilder app)
    {
        app.UseEndpoints(endpoints => endpoints.MapHealthChecks("/healthz/startup", new HealthCheckOptions
        {
            Predicate = check => check.Tags.Contains("Startup"),
            ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse
        }));

        app.UseEndpoints(endpoints => endpoints.MapHealthChecks("/healthz/live", new HealthCheckOptions
        {
            Predicate = _ => false,
            ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse
        }));

        app.UseEndpoints(endpoints => endpoints.MapHealthChecks("/healthz/ready", new HealthCheckOptions
        {
            ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse
        }));

        return app;
    }
}
