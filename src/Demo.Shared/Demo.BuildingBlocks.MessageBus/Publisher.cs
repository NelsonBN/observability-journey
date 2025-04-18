﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using BuildingBlocks.Contracts;
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
    IChannel channel) : IPublisher
{
    private readonly ILogger<Publisher> _logger = logger;
    private readonly MessageBusOptions _options = options.Value;
    private readonly IChannel _channel = channel;

    public Task Publish(params IEnumerable<Message> messages)
        => Task.WhenAll(messages.Select(Publish)); // TODO: to be implemented

    public async Task Publish(Message message)
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


        var properties = MessageBusFactory.CreateProperties(
            message.Type,
            message.MessageId.ToString());

        Propagators.DefaultTextMapPropagator.Inject(
            new(contextToInject, Baggage.Current),
            properties,
            (props, key, value) =>
            {
                props.Headers ??= new Dictionary<string, object?>();
                props.Headers[key] = value;
            });

        activity?
            .SetTag(MessageBusTelemetry.SemanticConventions.SYSTEM, "rabbitmq")
            .SetTag(MessageBusTelemetry.SemanticConventions.DESTINATION_KIND, "exchange")
            .SetTag(MessageBusTelemetry.SemanticConventions.OPERATION_TYPE, "publish")
            .SetTag(MessageBusTelemetry.SemanticConventions.DESTINATION_NAME, _options.ExchangeName)
            .SetTag(MessageBusTelemetry.SemanticConventions.MESSAGE_ID, properties.MessageId)
            .SetTag(MessageBusTelemetry.SemanticConventions.CORRELATION_ID, properties.CorrelationId)
            .SetTag(MessageBusTelemetry.SemanticConventions.ROUTING_KEY, message.Type)
            .AddMessage(message);

        await _channel.BasicPublishAsync(
            exchange: _options.ExchangeName,
            routingKey: message.Type,
            basicProperties: properties,
            mandatory: true,
            body: message.Serialize());

        _logger.LogInformation("[MESSAGE BUS][PUBLISHER] {MessageType} published", message.Type);
    }
}
