namespace BuildingBlocks.Observability;

internal static class TelemetrySemanticConventions
{
    public const string AT = "at";
    public const string MESSAGE_ID = "message_id";
    public const string CORRELATION_ID = "correlation_id";

    internal static class RabbitMQ
    {
        public const string ROUTING_KEY = "messaging.rabbitmq.routing_key";
        public const string DESTINATION = "messaging.destination";
        public const string DESTINATION_KIND = "messaging.destination_kind";
        public const string SYSTEM = "messaging.system";
    }
}
