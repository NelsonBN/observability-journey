using Notifications;

namespace Api.Users.Domain;

public interface INotificationsService
{
    Task<NotificationsTotalsResponse> GetNotificationsTotalsAsync(Guid userId, CancellationToken cancellationToken);
}
