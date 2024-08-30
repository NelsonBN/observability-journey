using Api.Notifications.Domain;
using BuildingBlocks.Events;
using MediatR;

namespace Api.Notifications.UseCases;

public sealed class SMSNotificationHandler(ILogger<SMSNotificationHandler> logger, INotificationsRepository repository) :
    INotificationHandler<SMSNotificationSentEvent>,
    INotificationHandler<SMSNotificationFailedEvent>
{
    private readonly ILogger<SMSNotificationHandler> _logger = logger;
    private readonly INotificationsRepository _repository = repository;

    public async Task Handle(SMSNotificationSentEvent domainEvent, CancellationToken cancellationToken)
    {
        _logger.LogInformation($"[MESSAGE BUS][WORKER][CONSUMER][HANDLER] {domainEvent.GetType().Name} received");

        var notification = await _repository.GetAsync(domainEvent.Id, cancellationToken);
        if(notification is null)
        {
            throw new NotificationNotFoundException(domainEvent.Id);
        }

        notification.SMSSent();

        await _repository.UpdateAsync(notification, cancellationToken);

        _logger.LogInformation($"[MESSAGE BUS][WORKER][CONSUMER][HANDLER] {domainEvent.GetType().Name} handled");
    }

    public async Task Handle(SMSNotificationFailedEvent domainEvent, CancellationToken cancellationToken)
    {
        _logger.LogInformation($"[MESSAGE BUS][WORKER][CONSUMER][HANDLER] {domainEvent.GetType().Name} received");

        var notification = await _repository.GetAsync(domainEvent.Id, cancellationToken);
        if(notification is null)
        {
            throw new NotificationNotFoundException(domainEvent.Id);
        }

        notification.SMSFailed();

        await _repository.UpdateAsync(notification, cancellationToken);

        _logger.LogInformation($"[MESSAGE BUS][WORKER][CONSUMER][HANDLER] {domainEvent.GetType().Name} handled");
    }
}
