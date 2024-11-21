using System;
using System.Threading;
using System.Threading.Tasks;
using Api.Notifications.Domain;
using Api.Notifications.DTOs;
using BuildingBlocks.Contracts.Abstractions;

namespace Api.Notifications.UseCases;

public sealed class SendNotificationCommand(
    INotificationsRepository repository,
    IUsersService service,
    IPublisher publisher)
{
    private readonly INotificationsRepository _repository = repository;
    private readonly IUsersService _service = service;
    private readonly IPublisher _publisher = publisher;

    public async Task<Guid> HandleAsync(NotificationRequest request, CancellationToken cancellationToken)
    {
        var user = await _service.GetUserAsync(request.UserId, cancellationToken);

        var notification = Notification.Create(
            request.UserId,
            request.Body,
            user.Email,
            user.Phone);

        await _repository.AddAsync(notification, cancellationToken);

        await _publisher.Publish(notification.GetDomainEvents());

        return notification.Id;
    }
}
