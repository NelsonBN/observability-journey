using System;
using System.Collections.Generic;
using System.Diagnostics;
using BuildingBlocks.Contracts.Abstractions;
using BuildingBlocks.Observability;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using OpenTelemetry;
using OpenTelemetry.Context.Propagation;
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

        using var activity = MessageBusTelemetry.Source.StartActivity($"Exchange {_options.ExchangeName}", ActivityKind.Producer);

        ActivityContext contextToInject = default;
        if(activity is not null)
        {
            contextToInject = activity.Context;
        }
        else if(Activity.Current is not null)
        {
            contextToInject = Activity.Current.Context;
        }

        var messageType = message.GetType().Name;
        var properties = _channel.CreateProperties(messageType);

        Propagators.DefaultTextMapPropagator.Inject(
            new(contextToInject, Baggage.Current),
            properties,
            (props, key, value) =>
            {
                props.Headers ??= new Dictionary<string, object>();
                props.Headers[key] = value;
            });

        activity?
            .SetTag(MessageBusTelemetry.SemanticConventions.SYSTEM, "rabbitmq")
            .SetTag(MessageBusTelemetry.SemanticConventions.DESTINATION_KIND, "exchange")
            .SetTag(MessageBusTelemetry.SemanticConventions.OPERATION_TYPE, "publish")
            .SetTag(MessageBusTelemetry.SemanticConventions.DESTINATION_NAME, _options.ExchangeName)
            .SetTag(MessageBusTelemetry.SemanticConventions.MESSAGE_ID, properties.MessageId)
            .SetTag(MessageBusTelemetry.SemanticConventions.CORRELATION_ID, properties.CorrelationId)
            .SetTag(MessageBusTelemetry.SemanticConventions.ROUTING_KEY, messageType)
            .AddMessage(message);

        _channel.BasicPublish(
            exchange: _options.ExchangeName,
            routingKey: messageType,
            basicProperties: properties,
            body: message.Serialize());

        _logger.LogInformation("[MESSAGE BUS][PUBLISHER] {MessageType} published", messageType);
    }
}
