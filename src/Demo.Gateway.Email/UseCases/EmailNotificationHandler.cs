using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using BuildingBlocks.Contracts.Abstractions;
using BuildingBlocks.Contracts.Events;
using BuildingBlocks.Observability;
using Microsoft.Extensions.Logging;

namespace Gateway.Email.UseCases;

public class EmailNotificationHandler(
    ILogger<EmailNotificationHandler> logger,
    IPublisher publisher)
    : IMessageHandler<EmailNotificationRequestedEvent>
{
    private readonly ILogger<EmailNotificationHandler> _logger = logger;
    private readonly IPublisher _publisher = publisher;

    public async Task HandleAsync(EmailNotificationRequestedEvent message, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("[NOTIFICATION][EMAIL][HANDLER] received");


        try
        {
            await _publisher.Publish(new EmailFeedbackEvent
            {
                Id = message.Id,
                Success = true
            });

            _logger.LogInformation("[NOTIFICATION][EMAIL][HANDLER] handled");
        }
        catch(Exception exception)
        {
            await _publisher.Publish(new EmailFeedbackEvent
            {
                Id = message.Id,
                Success = false
            });

            _logger.LogError(exception, "[NOTIFICATION][EMAIL][HANDLER]");
            Activity.Current.RegisterException(exception);
        }
    }
}
