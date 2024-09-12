using System;
using System.Diagnostics;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using BuildingBlocks.Contracts.Abstractions;
using BuildingBlocks.Observability;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using OpenTelemetry;
using OpenTelemetry.Context.Propagation;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace BuildingBlocks.MessageBus;

public sealed class Consumer<TMessage, THandler> : IHostedService
    where TMessage : IMessage
    where THandler : IMessageHandler<TMessage>
{
    private readonly ILogger<Consumer<TMessage, THandler>> _logger;
    private readonly IModel _channel;
    private readonly IServiceProvider _serviceProvider;
    private readonly string _exchangeName;
    private readonly string _queueName;

    private readonly AsyncEventingBasicConsumer _consumer;


    public Consumer(
        IServiceProvider serviceProvider,
        string exchangeName,
        string queueName)
    {
        _serviceProvider = serviceProvider;

        _exchangeName = exchangeName;
        _queueName = queueName;

        _logger = _serviceProvider.GetRequiredService<ILogger<Consumer<TMessage, THandler>>>();
        _channel = _serviceProvider.GetRequiredService<IModel>();

        _logger.LogInformation("[MESSAGE BUS][CONSUMER][{ExchangeName}][{QueueName}] Setting...", _exchangeName, _queueName);

        // Create Exchange
        _channel.ExchangeDeclare(
            exchange: exchangeName,
            type: ExchangeType.Topic,
            durable: true,
            autoDelete: false,
            arguments: null);

        // Create Queue
        _channel.QueueDeclare(
            queue: _queueName,
            durable: true,
            exclusive: false,
            autoDelete: false,
            arguments: null);

        // Attach Queue to Exchange
        _channel.QueueBind(
            queue: _queueName,
            exchange: exchangeName,
            routingKey: typeof(TMessage).Name);

        // Set QoS
        _channel.BasicQos(
            0,
            1, // Total message receive same time
            false); // [ false per consumer | true per channel ]

        _consumer = new(_channel);
        _consumer.Received += _listener;

        _logger.LogInformation("[MESSAGE BUS][CONSUMER][{ExchangeName}][{QueueName}] Set", _exchangeName, _queueName);
    }

    private async Task _listener(object sender, BasicDeliverEventArgs args)
    {
        MessageBusTelemetry.IncreaseRequest();

        Activity? activity = default;

        try
        {
            var parentContext = Propagators.DefaultTextMapPropagator.Extract(
                default,
                args.BasicProperties,
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


            activity = MessageBusTelemetry.Source.StartActivity(
                $"Consumer {_queueName}",
                ActivityKind.Consumer,
                parentContext.ActivityContext);

            activity?
                .SetTag(MessageBusTelemetry.SemanticConventions.SYSTEM, "rabbitmq")
                .SetTag(MessageBusTelemetry.SemanticConventions.DESTINATION_KIND, "queue")
                .SetTag(MessageBusTelemetry.SemanticConventions.OPERATION_TYPE, "receive")
                .SetTag(MessageBusTelemetry.SemanticConventions.DESTINATION_NAME, _queueName)
                .SetTag(MessageBusTelemetry.SemanticConventions.MESSAGE_ID, args.BasicProperties.MessageId)
                .SetTag(MessageBusTelemetry.SemanticConventions.CORRELATION_ID, args.BasicProperties.CorrelationId)
                .SetTag(MessageBusTelemetry.SemanticConventions.ROUTING_KEY, args.RoutingKey);

            var message = args.Deserialize<TMessage>()
                ?? throw new NotSupportedException($"The message type '{typeof(TMessage).Name}' is not supported by consummer");

            activity.AddMessage(message);


            await _dispatch(message);


            _channel.BasicAck(
                args.DeliveryTag,
                false);
        }
        catch(NotSupportedException exception)
        {
            _logger.LogError(
               exception,
               "[MESSAGE BUS][CONSUMER][{ExchangeName}][{QueueName}] Error deserializing message", _exchangeName, _queueName);

            activity.RegisterException(exception);

            _channel.BasicReject(
                args.DeliveryTag,
                false);
        }
        catch(Exception exception)
        {
            _logger.LogError(
                exception,
                "[MESSAGE BUS][CONSUMER][{ExchangeName}][{QueueName}] Error handling message", _exchangeName, _queueName);

            activity.RegisterException(exception);

            _channel.BasicNack(
                args.DeliveryTag,
                false,
                false);
        }
        finally
        {
            activity?.Dispose();
        }
    }

    private async Task _dispatch(TMessage message, CancellationToken cancellationToken = default)
    {
        using var scope = _serviceProvider.CreateScope();
        var handler = scope.ServiceProvider.GetRequiredService<THandler>();

        await handler.HandleAsync(
            message,
            cancellationToken);
    }


    public Task StartAsync(CancellationToken cancellationToken)
    {
        _logger.LogInformation("[MESSAGE BUS][CONSUMER][{ExchangeName}][{QueueName}] Starting...", _exchangeName, _queueName);

        _channel.BasicConsume(
            queue: _queueName,
            autoAck: false,
            consumer: _consumer);

        _logger.LogInformation("[MESSAGE BUS][CONSUMER][{ExchangeName}][{QueueName}] Started", _exchangeName, _queueName);

        return Task.CompletedTask;
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        _channel.Dispose();
        _logger.LogInformation("[MESSAGE BUS][CONSUMER][{ExchangeName}][{QueueName}] Stopped", _exchangeName, _queueName);

        return Task.CompletedTask;
    }
}
