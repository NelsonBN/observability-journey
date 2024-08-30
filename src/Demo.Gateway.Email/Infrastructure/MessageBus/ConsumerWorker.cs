using System.Diagnostics;
using System.Text.Json;
using BuildingBlocks.Events;
using BuildingBlocks.MessageBus;
using BuildingBlocks.Observability;
using MediatR;
using Microsoft.Extensions.Options;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace Gateway.Email.Infrastructure.MessageBus;

internal sealed partial class ConsumerWorker : IHostedService
{
    private readonly MessageBusOptions _options;
    private readonly ILogger<ConsumerWorker> _logger;
    private readonly IModel _channel;
    private readonly IServiceProvider _serviceProvider;

    private readonly AsyncEventingBasicConsumer _consumer;


    public ConsumerWorker(
        IOptions<MessageBusOptions> options,
        ILogger<ConsumerWorker> logger,
        IModel channel,
        IServiceProvider serviceProvider)
    {

        _options = options.Value;
        _logger = logger;
        _channel = channel;
        _serviceProvider = serviceProvider;

        _logger.LogInformation("[MESSAGE BROKER][CONSUMER] Setting...");

        _setup();

        _consumer = new(_channel);
        _consumer.Received += (_, arg) => _listener(arg);

        _logger.LogInformation("[MESSAGE BROKER][CONSUMER] Set");
    }

    private async Task _listener(BasicDeliverEventArgs args)
    {
        try
        {
            Telemetry.AddMessageBusRequest();
            var domainEvent = _deserialize(args);

            using var activity = Telemetry.Source
                .StartConsumerActivity(_options.QueueName, args.BasicProperties)
                .AddRoutingKey(args.RoutingKey)
                .AddMessage(domainEvent);


            await _dispatch(domainEvent);


            _channel.BasicAck(
                args.DeliveryTag,
                false);
        }
        catch(NotSupportedException exception)
        {
            _logger.LogError(
               exception,
               $"[MESSAGE BROKER][CONSUMER][DESERIALIZE]");

            Activity.Current.RegisterException(exception);

            _channel.BasicReject(
                args.DeliveryTag,
                false);
        }
        catch(Exception exception)
        {
            _logger.LogError(
                exception,
                $"[MESSAGE BROKER][CONSUMER][HANDLE]");

            Activity.Current.RegisterException(exception);

            _channel.BasicNack(
                args.DeliveryTag,
                false,
                false);
        }
    }


    private async Task _dispatch(DomainEvent domainEvent)
    {
        using var scope = _serviceProvider.CreateScope();

        var mediator = scope.ServiceProvider.GetRequiredService<IMediator>();

        await mediator.Publish(domainEvent);
    }

    private static DomainEvent _deserialize(BasicDeliverEventArgs args)
    {
        var messageType = args.GetMessageType();

        if(_supportedMessages.TryGetValue(messageType, out var type))
        {
            if(args.Deserialize(type) is DomainEvent message)
            {
                return message;
            }
        }

        throw new NotSupportedException($"The message type '{messageType}' is not supported by consummer");
    }



    public Task StartAsync(CancellationToken cancellationToken)
    {
        _logger.LogInformation("[MESSAGE BROKER][CONSUMER] Starting...");

        _channel.BasicConsume(
            queue: _options.QueueName,
            autoAck: false,
            consumer: _consumer);

        _logger.LogInformation("[MESSAGE BROKER][CONSUMER] Started");

        return Task.CompletedTask;
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        _logger.LogInformation("[MESSAGE BROKER][CONSUMER] Stopped");
        return Task.CompletedTask;
    }
}
