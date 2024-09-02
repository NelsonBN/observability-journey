using System.Diagnostics;
using System.Diagnostics.Metrics;
using System.Reflection;
using OpenTelemetry.Metrics;
using OpenTelemetry.Trace;

namespace BuildingBlocks.MessageBus;

public static class MessageBusTelemetry
{
    private const string NAME = "MessageBus";

    internal static readonly ActivitySource Source = new(
        name: NAME,
        version: Assembly.GetEntryAssembly()!.GetName().Version!.ToString());

    internal static readonly Meter Meter = new(
        name: NAME,
        version: Assembly.GetEntryAssembly()!.GetName().Version!.ToString());



    private static readonly Counter<int> _requestsCount = Meter.CreateCounter<int>("messagebus_requests_count", description: "Counts the number of MessageBus requests");
    public static void IncreaseRequest() => _requestsCount.Add(1);


    public static TracerProviderBuilder AddMessageBus(this TracerProviderBuilder builder)
        => builder.AddSource(NAME);

    public static MeterProviderBuilder AddMessageBus(this MeterProviderBuilder builder)
        => builder.AddMeter(NAME);


    public static class SemanticConventions
    {
        public const string MESSAGE_ID = "messaging.message.id";
        public const string CORRELATION_ID = "messaging.message.conversation_id";
        public const string DESTINATION_NAME = "messaging.destination.name";
        public const string DESTINATION_KIND = "messaging.destination_kind";
        public const string OPERATION_TYPE = "messaging.operation.type";
        public const string ROUTING_KEY = "messaging.rabbitmq.destination.routing_key";
        public const string SYSTEM = "messaging.system";
    }
}
