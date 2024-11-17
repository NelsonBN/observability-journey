using System;
using System.Threading;
using System.Threading.Tasks;
using Api.Notifications.Domain;
using Api.Notifications.DTOs;
using BuildingBlocks.Contracts.Exceptions;

namespace Api.Notifications.UseCases;

public sealed class GetNotificationQuery(INotificationsRepository repository)
{
    private readonly INotificationsRepository _repository = repository;

    public async Task<NotificationResponse> HandleAsync(Guid id, CancellationToken cancellationToken)
    {
        ExceptionFactory.ProbablyThrow<GetNotificationQuery>();

        var notification = await _repository.GetAsync(id, cancellationToken);
        if(notification is null)
        {
            throw new NotificationNotFoundException(id);
        }

        return notification;
    }
}
