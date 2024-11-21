using System;
using System.Diagnostics;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using BuildingBlocks.Contracts;
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

public sealed class Consumer<THandler> : IHostedService
    where THandler : IMessageHandler
{
    private readonly ILogger<Consumer<THandler>> _logger;
    private readonly IChannel _channel;
    private readonly IServiceProvider _serviceProvider;
    private readonly MessageBusOptions _options;

    private AsyncEventingBasicConsumer _consumer;


    public Consumer(
        IServiceProvider serviceProvider,
        MessageBusOptions options)
    {
        _serviceProvider = serviceProvider;
        _options = options;

        _logger = _serviceProvider.GetRequiredService<ILogger<Consumer<THandler>>>();
        _channel = _serviceProvider.GetRequiredService<IChannel>();

        _consumer = new(_channel);
        _consumer.ReceivedAsync += _listener;
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
                if(props.Headers is null)
                {
                    return [];
                }

                if(props.Headers.TryGetValue(key, out var value))
                {
                    var bytes = value as byte[];
                    return [Encoding.UTF8.GetString(bytes!)];
                }
                return [];
            });
            Baggage.Current = parentContext.Baggage;


            activity = MessageBusTelemetry.Source.StartActivity(
                $"Consumer {_options.QueueName}",
                ActivityKind.Consumer,
                parentContext.ActivityContext);

            activity?
                .SetTag(MessageBusTelemetry.SemanticConventions.SYSTEM, "rabbitmq")
                .SetTag(MessageBusTelemetry.SemanticConventions.DESTINATION_KIND, "queue")
                .SetTag(MessageBusTelemetry.SemanticConventions.OPERATION_TYPE, "receive")
                .SetTag(MessageBusTelemetry.SemanticConventions.DESTINATION_NAME, _options.QueueName)
                .SetTag(MessageBusTelemetry.SemanticConventions.MESSAGE_ID, args.BasicProperties.MessageId)
                .SetTag(MessageBusTelemetry.SemanticConventions.CORRELATION_ID, args.BasicProperties.CorrelationId)
                .SetTag(MessageBusTelemetry.SemanticConventions.ROUTING_KEY, args.RoutingKey);

            var message = args.Deserialize<Message>()
                ?? throw new NotSupportedException($"The message type '{args.GetMessageType()}' is not supported by consummer");

            activity.AddMessage(message);

            await _dispatch(message);

            await _channel.BasicAckAsync(
                args.DeliveryTag,
                false);
        }
        catch(NotSupportedException exception)
        {
            _logger.LogError(
                exception,
                "[MESSAGE BUS][CONSUMER][{ExchangeName}][{QueueName}] Error deserializing message", _options.ExchangeName, _options.QueueName);

            activity.RegisterException(exception);

            await _channel.BasicRejectAsync(
                args.DeliveryTag,
                false);
        }
        catch(Exception exception)
        {
            _logger.LogError(
                exception,
                "[MESSAGE BUS][CONSUMER][{ExchangeName}][{QueueName}] Error handling message", _options.ExchangeName, _options.QueueName);

            activity.RegisterException(exception);

            await _channel.BasicNackAsync(
                args.DeliveryTag,
                false,
                false);
        }
        finally
        {
            activity?.Dispose();
        }
    }

    private async Task _dispatch(Message message, CancellationToken cancellationToken = default)
    {
        using var scope = _serviceProvider.CreateScope();
        var handler = scope.ServiceProvider.GetRequiredService<THandler>();

        await handler.HandleAsync(
            message,
            cancellationToken);
    }


    public async Task StartAsync(CancellationToken cancellationToken)
    {
        _logger.LogInformation("[MESSAGE BUS][CONSUMER][{ExchangeName}][{QueueName}] Setting...", _options.ExchangeName, _options.QueueName);

        // Create Exchange
        await _channel.ExchangeDeclareAsync(
            exchange: _options.ExchangeName,
            type: ExchangeType.Topic,
            durable: true,
            autoDelete: false,
            arguments: null,
            cancellationToken: cancellationToken);

        // Create Queue
        await _channel.QueueDeclareAsync(
            queue: _options.QueueName,
            durable: true,
            exclusive: false,
            autoDelete: false,
            arguments: null,
            cancellationToken: cancellationToken);

        // Attach Queue to Exchange
        await _channel.QueueBindAsync(
            queue: _options.QueueName,
            exchange: _options.ExchangeName,
            routingKey: _options.RoutingKey,
            cancellationToken: cancellationToken);

        // Set QoS
        await _channel.BasicQosAsync(
            0,
            1, // Total message receive same time
            false,
            cancellationToken: cancellationToken); // [ false per consumer | true per channel ]

        _consumer = new(_channel);
        _consumer.ReceivedAsync += _listener;

        _logger.LogInformation("[MESSAGE BUS][CONSUMER][{ExchangeName}][{QueueName}] Set", _options.ExchangeName, _options.QueueName);

        await _channel.BasicConsumeAsync(
            queue: _options.QueueName,
            autoAck: false,
            consumer: _consumer,
            cancellationToken: cancellationToken);

        _logger.LogInformation("[MESSAGE BUS][CONSUMER][{ExchangeName}][{QueueName}] Started", _options.ExchangeName, _options.QueueName);
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        _channel.Dispose();
        _logger.LogInformation("[MESSAGE BUS][CONSUMER][{ExchangeName}][{QueueName}] Stopped", _options.ExchangeName, _options.QueueName);

        return Task.CompletedTask;
    }
}
