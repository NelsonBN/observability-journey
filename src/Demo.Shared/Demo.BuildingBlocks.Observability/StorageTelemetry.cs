using System.Diagnostics;
using System.Diagnostics.Metrics;
using System.Reflection;
using OpenTelemetry.Metrics;
using OpenTelemetry.Trace;

namespace BuildingBlocks.Observability;

public static class StorageTelemetry
{
    private const string NAME = "StorageService";

    public static readonly ActivitySource Source = new(
        name: NAME,
        version: Assembly.GetEntryAssembly()!.GetName().Version!.ToString());

    internal static readonly Meter Meter = new(
        name: NAME,
        version: Assembly.GetEntryAssembly()!.GetName().Version!.ToString());



    private static readonly Counter<int> _requestsCount = Meter.CreateCounter<int>("storage_requests_count", description: "Counts the number of MessageBus requests");
    public static void IncreaseRequest() => _requestsCount.Add(1);


    public static TracerProviderBuilder AddStorage(this TracerProviderBuilder builder)
        => builder.AddSource(NAME);

    public static MeterProviderBuilder AddStorage(this MeterProviderBuilder builder)
        => builder.AddMeter(NAME);


    public static class SemanticConventions
    {
        public const string CONTAINER = "storage.azure.container";
        public const string BLOB = "storage.azure.blob";
        public const string OPERATION_TYPE = "storage.operation.type";
        public const string SYSTEM = "storage.system";
    }
}
