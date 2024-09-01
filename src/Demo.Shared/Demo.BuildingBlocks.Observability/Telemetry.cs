using System.Diagnostics;
using System.Diagnostics.Metrics;

namespace BuildingBlocks.Observability;

public static class Telemetry
{
    public static readonly Meter Meter = new(AppDetails.Name);
    public static readonly ActivitySource Source = new(AppDetails.Name);


    private static readonly Counter<int> _requestsHttpCount = Meter.CreateCounter<int>("demo_http_requests_count", description: "Counts the number of HTTP requests");
    public static void AddHttpRequest() => _requestsHttpCount.Add(1);


    private static readonly Counter<int> _requestsGrpcCount = Meter.CreateCounter<int>("demo_grpc_requests_count", description: "Counts the number of gRPC requests");
    public static void AddGrpcRequest() => _requestsGrpcCount.Add(1);


    private static readonly Counter<int> _requestsMessageBusCount = Meter.CreateCounter<int>("demo_grpc_requests_count", description: "Counts the number of MessageBus requests");
    public static void AddMessageBusRequest() => _requestsMessageBusCount.Add(1);
}
