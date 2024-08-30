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
        var parentContext = Propagators.DefaultTextMapPropagator.Extract(default, properties, (props, key) =>
        {
            if(props.Headers.TryGetValue(key, out var value))
            {
                var bytes = value as byte[];
                return [Encoding.UTF8.GetString(bytes!)];
            }

            return [];
        });
        Baggage.Current = parentContext.Baggage;

        var activity = activitySource.StartActivity($"Consumer {queueName}", ActivityKind.Consumer, parentContext.ActivityContext);
        if(activity is null)
        {
            return activity;
        }

        activity
            .SetTag(DiagnosticsNames.RabbitMQ.SYSTEM, "rabbitmq")
            .SetTag(DiagnosticsNames.RabbitMQ.DESTINATION_KIND, "queue")
            .SetTag(DiagnosticsNames.RabbitMQ.DESTINATION, queueName)
            .SetTag(DiagnosticsNames.MESSAGE_ID, properties.MessageId)
            .SetTag(DiagnosticsNames.CORRELATION_ID, properties.CorrelationId)
            .AddAt();

        return activity;
    }

    public static Activity? StartProducerActivity(this ActivitySource activitySource, string exchangeName, IBasicProperties properties)
    {
        var activity = activitySource.StartActivity($"Exchange {exchangeName}", ActivityKind.Producer);
        if(activity is null)
        {
            return activity;
        }

        activity
            .SetTag(DiagnosticsNames.RabbitMQ.SYSTEM, "rabbitmq")
            .SetTag(DiagnosticsNames.RabbitMQ.DESTINATION_KIND, "exchange")
            .SetTag(DiagnosticsNames.RabbitMQ.DESTINATION, exchangeName)
            .SetTag(DiagnosticsNames.MESSAGE_ID, properties.MessageId)
            .SetTag(DiagnosticsNames.CORRELATION_ID, properties.CorrelationId)
            .AddAt();

        var contextToInject = activity.Context;

        Propagators.DefaultTextMapPropagator
            .Inject(new PropagationContext(contextToInject, Baggage.Current), properties, (props, key, value) =>
            {
                props.Headers ??= new Dictionary<string, object>();
                props.Headers[key] = value;
            });

        return activity;
    }

    public static Activity? AddRoutingKey(this Activity? activity, string routingKey)
        => activity?.SetTag(DiagnosticsNames.RabbitMQ.ROUTING_KEY, routingKey);

    public static Activity? AddMessage<TMessage>(this Activity? activity, TMessage message)
        => activity?.SetTag("message", JsonSerializer.Serialize(message));

    public static Activity? AddAt(this Activity? activity)
        => activity?.SetTag(DiagnosticsNames.AT, DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss.ffffff"));

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
