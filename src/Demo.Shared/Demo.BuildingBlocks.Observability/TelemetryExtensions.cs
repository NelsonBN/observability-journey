using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Text.Json;
using OpenTelemetry;
using OpenTelemetry.Context.Propagation;
using OpenTelemetry.Trace;
using RabbitMQ.Client;

namespace BuildingBlocks.Observability;

public static class TelemetryExtensions
{
    public static Activity? StartHttpActivity(this ActivitySource activitySource, string description)
        => activitySource.StartActivity(description, ActivityKind.Server);

    public static Activity? StartConsumerActivity(this ActivitySource activitySource, string queueName, IBasicProperties properties)
    {
        var parentContext = Propagators.DefaultTextMapPropagator.Extract(
            default,
            properties,
            (props, key) =>
            {
                if(props.Headers.TryGetValue(key, out var value))
                {
                    var bytes = value as byte[];
                    return [Encoding.UTF8.GetString(bytes!)];
                }
                return [];
            });
        Baggage.Current = parentContext.Baggage;


        using var activity = activitySource.StartActivity($"Consumer {queueName}", ActivityKind.Consumer, parentContext.ActivityContext);
        if(activity is null)
        {
            return activity;
        }

        activity
            .SetTag(TelemetrySemanticConventions.RabbitMQ.SYSTEM, "rabbitmq")
            .SetTag(TelemetrySemanticConventions.RabbitMQ.DESTINATION_KIND, "queue")
            .SetTag(TelemetrySemanticConventions.RabbitMQ.OPERATION_TYPE, "receive")
            .SetTag(TelemetrySemanticConventions.RabbitMQ.DESTINATION_NAME, queueName)
            .SetTag(TelemetrySemanticConventions.RabbitMQ.MESSAGE_ID, properties.MessageId)
            .SetTag(TelemetrySemanticConventions.RabbitMQ.CORRELATION_ID, properties.CorrelationId)
            .AddAt();

        return activity;
    }

    public static Activity? StartProducerActivity(this ActivitySource activitySource, string exchangeName, IBasicProperties properties)
    {
        using var activity = activitySource.StartActivity($"Exchange {exchangeName}", ActivityKind.Producer);

        ActivityContext contextToInject = default;
        if(activity is not null)
        {
            contextToInject = activity.Context;
        }
        else if(Activity.Current is not null)
        {
            contextToInject = Activity.Current.Context;
        }

        Propagators.DefaultTextMapPropagator.Inject(
            new(contextToInject, Baggage.Current),
            properties,
            (props, key, value) =>
            {
                props.Headers ??= new Dictionary<string, object>();
                props.Headers[key] = value;
            });

        activity?
            .SetTag(TelemetrySemanticConventions.RabbitMQ.SYSTEM, "rabbitmq")
            .SetTag(TelemetrySemanticConventions.RabbitMQ.DESTINATION_KIND, "exchange")
            .SetTag(TelemetrySemanticConventions.RabbitMQ.OPERATION_TYPE, "publish")
            .SetTag(TelemetrySemanticConventions.RabbitMQ.DESTINATION_NAME, exchangeName)
            .SetTag(TelemetrySemanticConventions.RabbitMQ.MESSAGE_ID, properties.MessageId)
            .SetTag(TelemetrySemanticConventions.RabbitMQ.CORRELATION_ID, properties.CorrelationId)
            .AddAt();

        return activity;
    }

    public static Activity? AddRoutingKey(this Activity? activity, string routingKey)
        => activity?.SetTag(TelemetrySemanticConventions.RabbitMQ.ROUTING_KEY, routingKey);

    public static Activity? AddMessage<TMessage>(this Activity? activity, TMessage message)
        => activity?.SetTag("message", JsonSerializer.Serialize(message));

    public static Activity? AddAt(this Activity? activity)
        => activity?.SetTag(TelemetrySemanticConventions.AT, DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss.ffffff"));

    public static Activity? RegisterException<TException>(this Activity? activity, TException exception)
        where TException : Exception
    {
        if(activity is null)
        {
            return activity;
        }

        activity.SetStatus(ActivityStatusCode.Error, exception.Message);
        activity.RecordException(exception);

        return activity;
    }

    public static Activity? RegisterValidation<TValidation>(this Activity? activity, TValidation validation)
        where TValidation : Exception
    {
        if(activity is null)
        {
            return activity;
        }

        activity.AddEvent(new(
            typeof(TValidation).Name,
            tags: new([
                new("message", validation.Message)]
            )));

        return activity;
    }
}
