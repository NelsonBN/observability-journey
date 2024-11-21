using RabbitMQ.Client;

namespace BuildingBlocks.MessageBus;

public static class MessageBusFactory
{
    public static BasicProperties CreateProperties(string messageType, string? messageId = null)
        => new BasicProperties()
            .SetAppId()
            .SetCorrelationId()
            .SetMessageId(messageId)
            .SetContentTypeJson()
            .SetEncodingUTF8()
            .SetMessageType(messageType);
}
