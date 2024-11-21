using System;
using System.Threading;
using System.Threading.Tasks;
using BuildingBlocks.Contracts;
using BuildingBlocks.Contracts.Abstractions;
using Microsoft.Extensions.Logging;

namespace Gateway.SMS.UseCases;

public sealed class SMSNotificationHandler(
    ILogger<SMSNotificationHandler> logger,
    IPublisher publisher) : IMessageHandler
{
    private readonly ILogger<SMSNotificationHandler> _logger = logger;
    private readonly IPublisher _publisher = publisher;

    public async Task HandleAsync(Message message, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("[HANDLER][NOTIFICATION][SMS] received");

        var phone = Convert.ToString(message.Data["Phone"]);
        var body = Convert.ToString(message.Data["Body"]);

        var delay = Random.Shared.Next(50, 1000);

        await Task.Delay(delay, cancellationToken)
            .ContinueWith(async task =>
            {
                if(delay % 6 == 0)
                {
                    await _publisher.Publish(new Message
                    {
                        Context = "notifications",
                        Type = "sms.response",
                        AggregateId = message.AggregateId,
                        Data = new()
                        {
                            ["Sent"] = "false"
                        }
                    });

                    _logger.LogError("[HANDLER][NOTIFICATION][SMS][{Phone}] {Body}", phone, body);
                }

                else
                {
                    await _publisher.Publish(new Message
                    {
                        Context = "notifications",
                        Type = "sms.response",
                        AggregateId = message.AggregateId,
                        Data = new()
                        {
                            ["Sent"] = "true"
                        }
                    });

                    _logger.LogInformation("[HANDLER][NOTIFICATION][SMS][{Phone}] {Body}", phone, body);
                }

            }, cancellationToken);
    }
}
