using System;
using System.Threading;
using System.Threading.Tasks;
using BuildingBlocks.Contracts.Notifications;

namespace Api.Users.Domain;

public interface INotificationsService
{
    Task<NotificationsTotalsResponse> GetNotificationsTotalsAsync(Guid userId, CancellationToken cancellationToken);
}
