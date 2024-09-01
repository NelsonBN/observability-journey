using System;
using System.Threading;
using System.Threading.Tasks;
using BuildingBlocks.Contracts.Abstractions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
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

        _logger.LogInformation("[RABBITMQ][CONSUMER][{ExchangeName}][{QueueName}] Setting...", _exchangeName, _queueName);

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

        _logger.LogInformation("[RABBITMQ][CONSUMER][{ExchangeName}][{QueueName}] Set", _exchangeName, _queueName);
    }

    private async Task _listener(object sender, BasicDeliverEventArgs args)
    {
        try
        {
            //Telemetry.AddMessageBusRequest(); // TODO: working here

            var message = args.Deserialize<TMessage>()
                ?? throw new NotSupportedException($"The message type '{typeof(TMessage).Name}' is not supported by consummer");

            // TODO: working here
            //using var activity = Telemetry.Source
            //    .StartConsumerActivity(_queueName, args.BasicProperties)
            //    .AddRoutingKey(args.RoutingKey)
            //    .AddMessage(domainEvent);

            await _dispatch(message);

            _channel.BasicAck(
                args.DeliveryTag,
                false);
        }
        catch(NotSupportedException exception)
        {
            _logger.LogError(
               exception,
               "[RABBITMQ][CONSUMER][{ExchangeName}][{QueueName}] Error deserializing message", _exchangeName, _queueName);

            _channel.BasicReject(
                args.DeliveryTag,
                false);
        }
        catch(Exception exception)
        {
            _logger.LogError(
                exception,
                "[RABBITMQ][CONSUMER][{ExchangeName}][{QueueName}] Error handling message", _exchangeName, _queueName);

            _channel.BasicNack(
                args.DeliveryTag,
                false,
                false);
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
        _logger.LogInformation("[RABBITMQ][CONSUMER][{ExchangeName}][{QueueName}] Starting...", _exchangeName, _queueName);

        _channel.BasicConsume(
            queue: _queueName,
            autoAck: false,
            consumer: _consumer);

        _logger.LogInformation("[RABBITMQ][CONSUMER][{ExchangeName}][{QueueName}] Started", _exchangeName, _queueName);

        return Task.CompletedTask;
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        _channel.Dispose();
        _logger.LogInformation("[RABBITMQ][CONSUMER][{ExchangeName}][{QueueName}] Stopped", _exchangeName, _queueName);

        return Task.CompletedTask;
    }
}
