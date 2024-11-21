using System;
using System.Threading;
using System.Threading.Tasks;
using Api.Notifications.Domain;
using BuildingBlocks.Contracts;
using BuildingBlocks.Contracts.Abstractions;
using Microsoft.Extensions.Logging;

namespace Api.Notifications.UseCases;

public sealed class EmailResponseHandler(
    ILogger<EmailResponseHandler> logger,
    INotificationsRepository repository) : IMessageHandler
{
    private readonly ILogger<EmailResponseHandler> _logger = logger;
    private readonly INotificationsRepository _repository = repository;

    public async Task HandleAsync(Message message, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("[HANDLER][EMAIL][RESPONSE] received");

        var sent = Convert.ToBoolean(Convert.ToString(message.Data["Sent"]));

        var notification = await _repository.GetAsync(message.AggregateId, cancellationToken);
        if(notification is null)
        {
            throw new NotificationNotFoundException(message.AggregateId);
        }

        if(sent)
        {
            notification.EmailSent();
        }
        else
        {
            notification.EmailFailed();
        }

        await _repository.UpdateAsync(notification, cancellationToken);

        _logger.LogInformation("[HANDLER][EMAIL][RESPONSE] Sent={Sent}", sent);
    }
}
