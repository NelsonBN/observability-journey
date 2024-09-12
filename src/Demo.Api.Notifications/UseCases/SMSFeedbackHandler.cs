using System.Threading;
using System.Threading.Tasks;
using Api.Notifications.Domain;
using BuildingBlocks.Contracts.Abstractions;
using BuildingBlocks.Contracts.Events;
using Microsoft.Extensions.Logging;

namespace Api.Notifications.UseCases;

public class SMSFeedbackHandler(ILogger<SMSFeedbackHandler> logger, INotificationsRepository repository) : IMessageHandler<SMSFeedbackEvent>
{
    private readonly ILogger<SMSFeedbackHandler> _logger = logger;
    private readonly INotificationsRepository _repository = repository;

    public async Task HandleAsync(SMSFeedbackEvent message, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("[HANDLER][SMS][FEEDBACK] received");

        var notification = await _repository.GetAsync(message.Id, cancellationToken);
        if(notification is null)
        {
            throw new NotificationNotFoundException(message.Id);
        }

        if(message.Success)
        {
            notification.SMSSent();
        }
        else
        {
            notification.SMSFailed();
        }

        await _repository.UpdateAsync(notification, cancellationToken);

        _logger.LogInformation("[HANDLER][SMS][FEEDBACK] handled");
    }
}
