namespace Api.Notifications.Domain;

public interface INotificationsRepository
{
    Task<List<Notification>> ListAsync(CancellationToken cancellationToken = default);
    Task<Notification?> GetAsync(Guid id, CancellationToken cancellationToken = default);
    Task<NotificationsTotals?> GetTotalsAsync(Guid userId, CancellationToken cancellationToken = default);
    Task AddAsync(Notification notification, CancellationToken cancellationToken = default);
    Task UpdateAsync(Notification notification, CancellationToken cancellationToken = default);
}
