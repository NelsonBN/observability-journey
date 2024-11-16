using RabbitMQ.Client;

namespace BuildingBlocks.MessageBus;

public static class MessageBusFactory
{
    public static BasicProperties CreateProperties(string messageType)
        => new BasicProperties()
            .SetAppId()
            .SetCorrelationId()
            .SetMessageId()
            .SetContentTypeJson()
            .SetEncodingUTF8()
            .SetMessageType(messageType);
}
