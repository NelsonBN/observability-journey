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
        _logger.LogInformation("[HANDLER][SMS] received");

        var delay = Random.Shared.Next(50, 1000);

        await Task.Delay(delay, cancellationToken)
            .ContinueWith(task =>
            {
                if(delay % 6 == 0)
                {
                    _publisher.Publish(new SMSFeedbackEvent
                    {
                        Id = message.Id,
                        Success = false
                    });

                    _logger.LogError("[HANDLER][SMS] failed");
                }

                else
                {
                    _publisher.Publish(new SMSFeedbackEvent
                    {
                        Id = message.Id,
                        Success = true
                    });

                    _logger.LogInformation("[HANDLER][SMS] handled");
                }

            }, cancellationToken);
    }
}
