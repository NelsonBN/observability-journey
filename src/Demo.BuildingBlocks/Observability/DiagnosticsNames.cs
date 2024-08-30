namespace Common.Observability;

internal static class DiagnosticsNames
{
    public const string AT = "At";
    public const string MESSAGE_ID = "MessageId";
    public const string CORRELATION_ID = "CorrelationId";

    internal static class RabbitMQ
    {
        public const string ROUTING_KEY = "messaging.rabbitmq.routing_key";
        public const string DESTINATION = "messaging.destination";
        public const string DESTINATION_KIND = "messaging.destination_kind";
        public const string SYSTEM = "messaging.system";
    }
}
