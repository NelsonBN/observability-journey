using System;
using System.Threading;
using System.Threading.Tasks;
using BuildingBlocks.Contracts.Abstractions;
using BuildingBlocks.Contracts.Events;
using Gateway.Email.Domain;
using Microsoft.Extensions.Logging;

namespace Gateway.Email.UseCases;

public class EmailNotificationHandler(
    ILogger<EmailNotificationHandler> logger,
    IStorageService storage,
    IPublisher publisher)
    : IMessageHandler<EmailNotificationRequestedEvent>
{
    private readonly ILogger<EmailNotificationHandler> _logger = logger;
    private readonly IStorageService _storage = storage;
    private readonly IPublisher _publisher = publisher;

    public async Task HandleAsync(EmailNotificationRequestedEvent message, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("[HANDLER][EMAIL] received");

        var delay = Random.Shared.Next(50, 1000);

        await Task.Delay(delay, cancellationToken)
            .ContinueWith(task =>
            {
                if(delay % 6 == 0)
                {
                    _publisher.Publish(new EmailFeedbackEvent
                    {
                        Id = message.Id,
                        Success = false
                    });

                    _logger.LogError("[HANDLER][EMAIL] failed");
                }

                else
                {
                    _publisher.Publish(new EmailFeedbackEvent
                    {
                        Id = message.Id,
                        Success = true
                    });

                    _logger.LogInformation("[HANDLER][EMAIL] handled");
                }

            }, cancellationToken);
    }
}
