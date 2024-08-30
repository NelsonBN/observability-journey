using OpenTelemetry.Resources;

namespace BuildingBlocks.Observability;

public static class TelemetryFactory
{
    // It is important declarate has a action, because logging configuration and tracing/metrics configuration don't share this by themselves
    public static readonly Action<ResourceBuilder> Configure = (resourceBuilder)
        => resourceBuilder
            .AddService(
                serviceName: AppDetails.Name,
                serviceVersion: AppDetails.Version,
                serviceInstanceId: AppDetails.ServiceId)
            .AddTelemetrySdk();

    public static ResourceBuilder CreateResource()
    {
        var resourceBuilder = ResourceBuilder.CreateDefault();
        Configure(resourceBuilder);
        return resourceBuilder;
    }
}
