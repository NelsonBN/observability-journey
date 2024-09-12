using System.Threading;
using System.Threading.Tasks;
using Api.Notifications.Domain;
using BuildingBlocks.Contracts.Abstractions;
using BuildingBlocks.Contracts.Events;
using Microsoft.Extensions.Logging;

namespace Api.Notifications.UseCases;

public class EmailFeedbackHandler(ILogger<EmailFeedbackHandler> logger, INotificationsRepository repository)
    : IMessageHandler<EmailFeedbackEvent>
{
    private readonly ILogger<EmailFeedbackHandler> _logger = logger;
    private readonly INotificationsRepository _repository = repository;

    public async Task HandleAsync(EmailFeedbackEvent message, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("[HANDLER][EMAIL][FEEDBACK] received");

        var notification = await _repository.GetAsync(message.Id, cancellationToken);
        if(notification is null)
        {
            throw new NotificationNotFoundException(message.Id);
        }

        if(message.Success)
        {
            notification.EmailSent();
        }
        else
        {
            notification.EmailFailed();
        }

        await _repository.UpdateAsync(notification, cancellationToken);

        _logger.LogInformation("[HANDLER][EMAIL][FEEDBACK] handled");
    }
}
