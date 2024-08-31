namespace BuildingBlocks.Observability;

public static class TelemetrySemanticConventions
{
    public const string AT = "at";

    public static class RabbitMQ
    {
        public const string MESSAGE_ID = "messaging.message.id";
        public const string CORRELATION_ID = "messaging.message.conversation_id";
        public const string DESTINATION_NAME = "messaging.destination.name";
        public const string DESTINATION_KIND = "messaging.destination_kind";
        public const string OPERATION_TYPE = "messaging.operation.type";
        public const string ROUTING_KEY = "messaging.rabbitmq.destination.routing_key";
        public const string SYSTEM = "messaging.system";
    }

    public static class AzureStorage
    {
        public const string CONTAINER = "storage.azure.container";
        public const string BLOB = "storage.azure.blob";
        public const string OPERATION_TYPE = "storage.operation.type";
        public const string SYSTEM = "storage.system";
    }
}
