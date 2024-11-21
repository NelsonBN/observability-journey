using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using BuildingBlocks.Contracts;
using BuildingBlocks.Contracts.Abstractions;
using BuildingBlocks.Observability;
using Microsoft.Extensions.Logging;

namespace Gateway.Email.UseCases;

public sealed class EmailNotificationHandler(
    ILogger<EmailNotificationHandler> logger,
    IPublisher publisher) : IMessageHandler
{
    private readonly ILogger<EmailNotificationHandler> _logger = logger;
    private readonly IPublisher _publisher = publisher;

    public async Task HandleAsync(Message message, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("[HANDLER][NOTIFICATION][EMAIL] received");

        var email = Convert.ToString(message.Data["Email"]);
        var body = Convert.ToString(message.Data["Body"]);

        try
        {
            await _publisher.Publish(new Message
            {
                Context = "notifications",
                Type = "email.response",
                AggregateId = message.AggregateId,
                Data = new()
                {
                    ["Sent"] = "true"
                }
            });

            _logger.LogInformation("[HANDLER][NOTIFICATION][EMAIL][{Email}] {Body}", email, body);
        }
        catch(Exception exception)
        {
            await _publisher.Publish(new Message
            {
                Context = "notifications",
                Type = "email.response",
                AggregateId = message.AggregateId,
                Data = new()
                {
                    ["Sent"] = "false"
                }
            });

            _logger.LogError(exception, "[HANDLER][NOTIFICATION][EMAIL][{Email}] {Body}", email, body);
            Activity.Current.RegisterException(exception);
        }
    }
}
