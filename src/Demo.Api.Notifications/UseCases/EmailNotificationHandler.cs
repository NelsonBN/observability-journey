using Api.Notifications.Domain;
using Common.Events;
using MediatR;

namespace Api.Notifications.UseCases;

public sealed class EmailNotificationHandler(ILogger<EmailNotificationHandler> logger, INotificationsRepository repository) :
    INotificationHandler<EmailNotificationSentEvent>,
    INotificationHandler<EmailNotificationFailedEvent>
{
    private readonly ILogger<EmailNotificationHandler> _logger = logger;
    private readonly INotificationsRepository _repository = repository;

    public async Task Handle(EmailNotificationSentEvent domainEvent, CancellationToken cancellationToken)
    {
        _logger.LogInformation($"[MESSAGE BUS][WORKER][CONSUMER][HANDLER] {domainEvent.GetType().Name} received");

        var notification = await _repository.GetAsync(domainEvent.Id, cancellationToken);
        if(notification is null)
        {
            throw new NotificationNotFoundException(domainEvent.Id);
        }

        notification.EmailSent();

        await _repository.UpdateAsync(notification, cancellationToken);

        _logger.LogInformation($"[MESSAGE BUS][WORKER][CONSUMER][HANDLER] {domainEvent.GetType().Name} handled");
    }

    public async Task Handle(EmailNotificationFailedEvent domainEvent, CancellationToken cancellationToken)
    {
        _logger.LogInformation($"[MESSAGE BUS][WORKER][CONSUMER][HANDLER] {domainEvent.GetType().Name} received");

        var notification = await _repository.GetAsync(domainEvent.Id, cancellationToken);
        if(notification is null)
        {
            throw new NotificationNotFoundException(domainEvent.Id);
        }

        notification.EmailFailed();

        await _repository.UpdateAsync(notification, cancellationToken);

        _logger.LogInformation($"[MESSAGE BUS][WORKER][CONSUMER][HANDLER] {domainEvent.GetType().Name} handled");
    }
}
