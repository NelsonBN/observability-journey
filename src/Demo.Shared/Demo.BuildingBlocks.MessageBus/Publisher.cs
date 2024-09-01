using System;
using BuildingBlocks.Contracts.Abstractions;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using RabbitMQ.Client;

namespace BuildingBlocks.MessageBus;

internal sealed class Publisher(
    ILogger<Publisher> logger,
    IOptions<MessageBusOptions> options,
    IModel channel) : IPublisher
{
    private readonly ILogger<Publisher> _logger = logger;
    private readonly MessageBusOptions _options = options.Value;
    private readonly IModel _channel = channel;

    public void Publish<TEvent>(params TEvent[] messages)
        where TEvent : IMessage
    {
        foreach(var message in messages)
        {
            Publish(message);
        }
    }

    public void Publish<TEvent>(TEvent message)
        where TEvent : IMessage
    {
        ArgumentNullException.ThrowIfNull(message);

        var messageType = message.GetType().Name;

        var properties = _channel.CreateProperties(messageType);

        // TODO: to finish
        //using var activity = Telemetry.Source
        //    .StartProducerActivity(_options.ExchangeName, properties)
        //    .AddRoutingKey(messageType)
        //    .AddMessage(domainEvent);


        _channel.BasicPublish(
            exchange: _options.ExchangeName,
            routingKey: messageType,
            basicProperties: properties,
            body: message.Serialize());

        _logger.LogInformation("[RABBITMQ][PUBLISHER] {MessageType} published", messageType);
    }
}
