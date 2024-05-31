using Common.Events;
using Common.MessageBus;
using Common.Observability;
using Microsoft.Extensions.Options;
using RabbitMQ.Client;

namespace Gateway.Email.Infrastructure.MessageBus;

internal sealed class MessageBusServer(
    ILogger<MessageBusServer> logger,
    IOptions<MessageBusOptions> options,
    IModel channel)
: IMessageBus
{
    private readonly ILogger<MessageBusServer> _logger = logger;
    private readonly MessageBusOptions _options = options.Value;
    private readonly IModel _channel = channel;

    public void Publish<TMessage>(params TMessage[] domainEvents)
        where TMessage : DomainEvent
    {
        foreach(var domainEvent in domainEvents)
        {
            Publish(domainEvent);
        }
    }

    public void Publish<TMessage>(TMessage domainEvent)
        where TMessage : DomainEvent
    {
        ArgumentNullException.ThrowIfNull(domainEvent);

        var messageType = domainEvent.GetType().Name;

        var properties = _channel.CreateProperties(messageType);


        using var activity = Diagnostic.Source
            .StartProducerActivity(_options.ExchangeName, properties)
            .AddRoutingKey(messageType)
            .AddMessage(domainEvent);


        _channel.BasicPublish(
            exchange: _options.ExchangeName,
            routingKey: messageType,
            basicProperties: properties,
            body: domainEvent.Serialize());

        _logger.LogInformation("[MESSAGE BUS][PUBLISHER] {MessageType} published", messageType);
    }
}
