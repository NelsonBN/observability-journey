using System;
using System.Threading;
using System.Threading.Tasks;
using BuildingBlocks.Contracts.Abstractions;
using BuildingBlocks.Contracts.Events;
using Microsoft.Extensions.Logging;

namespace Gateway.SMS.UseCases;

public class SMSNotificationHandler(ILogger<SMSNotificationHandler> logger, IPublisher publisher)
    : IMessageHandler<SMSNotificationRequestedEvent>
{
    private readonly ILogger<SMSNotificationHandler> _logger = logger;
    private readonly IPublisher _publisher = publisher;

    public async Task HandleAsync(SMSNotificationRequestedEvent message, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("[NOTIFICATION][SMS][HANDLER] received");

        var delay = Random.Shared.Next(50, 1000);

        await Task.Delay(delay, cancellationToken)
            .ContinueWith(async task =>
            {
                if(delay % 6 == 0)
                {
                    await _publisher.Publish(new SMSFeedbackEvent
                    {
                        Id = message.Id,
                        Success = false
                    });

                    _logger.LogError("[NOTIFICATION][SMS][HANDLER] failed");
                }

                else
                {
                    await _publisher.Publish(new SMSFeedbackEvent
                    {
                        Id = message.Id,
                        Success = true
                    });

                    _logger.LogInformation("[NOTIFICATION][SMS][HANDLER] handled");
                }

            }, cancellationToken);
    }
}
