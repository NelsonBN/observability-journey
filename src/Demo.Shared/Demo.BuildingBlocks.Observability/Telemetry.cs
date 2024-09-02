using System.Diagnostics;
using System.Diagnostics.Metrics;
using System.Reflection;

namespace BuildingBlocks.Observability;

public static class Telemetry
{
    private static readonly string _name = Assembly.GetEntryAssembly()!.GetName().Name!;

    public static readonly Meter Meter = new(_name);
    public static readonly ActivitySource Source = new(_name);


    private static readonly Counter<int> _requestsHttpCount = Meter.CreateCounter<int>("http_requests_count", description: "Counts the number of HTTP requests");
    public static void IncreaseHttpRequest() => _requestsHttpCount.Add(1);


    private static readonly Counter<int> _requestsGrpcCount = Meter.CreateCounter<int>("grpc_requests_count", description: "Counts the number of gRPC requests");
    public static void IncreaseGrpcRequest() => _requestsGrpcCount.Add(1);
}
