using BuildingBlocks.Events;
using BuildingBlocks.MessageBus;
using Gateway.Email.Domain;
using MediatR;

namespace Gateway.Email.UseCases;

public sealed class EmailNotificationRequestedHandler(
    ILogger<EmailNotificationRequestedHandler> logger,
    IMessageBus messageBus,
    IStorageService storage)
: INotificationHandler<EmailNotificationRequestedEvent>
{
    private readonly ILogger<EmailNotificationRequestedHandler> _logger = logger;
    private readonly IMessageBus _messageBus = messageBus;
    private readonly IStorageService _storage = storage;

    public async Task Handle(EmailNotificationRequestedEvent domainEvent, CancellationToken cancellationToken)
    {
        _logger.LogInformation($"[MESSAGE BUS][WORKER][CONSUMER][HANDLER] {domainEvent.GetType().Name} received");

        var delay = Random.Shared.Next(50, 1000);

        await Task.Delay(delay, cancellationToken)
            .ContinueWith(task =>
            {
                if(delay % 6 == 0)
                {
                    _messageBus.Publish(new EmailNotificationFailedEvent
                    {
                        Id = domainEvent.Id,
                    });

                    _logger.LogError($"[MESSAGE BUS][WORKER][CONSUMER][HANDLER] {domainEvent.GetType().Name} failed");
                }

                else
                {
                    _messageBus.Publish(new EmailNotificationSentEvent
                    {
                        Id = domainEvent.Id,
                    });

                    _logger.LogInformation($"[MESSAGE BUS][WORKER][CONSUMER][HANDLER] {domainEvent.GetType().Name} handled");
                }

            }, cancellationToken);
    }
}
