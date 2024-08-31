using Api.Notifications.Domain;
using Microsoft.EntityFrameworkCore;

namespace Api.Notifications.Infrastructure.Database;

public sealed class NotificationsRepository(DataContext context) : INotificationsRepository
{
    private readonly DataContext _context = context;

    public Task<List<Notification>> ListAsync(CancellationToken cancellationToken = default)
        => _context.Notifications.ToListAsync(cancellationToken);

    public Task<Notification?> GetAsync(Guid id, CancellationToken cancellationToken = default)
        => _context.Notifications.SingleOrDefaultAsync(s => s.Id == id, cancellationToken);

    public async Task<NotificationsTotals?> GetTotalsAsync(Guid userId, CancellationToken cancellationToken = default)
        => await _context.Notifications
            .Where(n => n.UserId == userId)
            .GroupBy(n => n.UserId)
            .Select(g => new NotificationsTotals
            {
                UserId = g.Key,

                TotalEmailsNoSent = g.Count(n => n.EmailNotificationStatus == NotificationStatus.NoSent),
                TotalEmailsPending = g.Count(n => n.EmailNotificationStatus == NotificationStatus.Pending),
                TotalEmailsSent = g.Count(n => n.EmailNotificationStatus == NotificationStatus.Sent),
                TotalEmailsFailed = g.Count(n => n.EmailNotificationStatus == NotificationStatus.Failed),

                TotalSMSsNoSent = g.Count(n => n.PhoneNotificationStatus == NotificationStatus.NoSent),
                TotalSMSsPending = g.Count(n => n.PhoneNotificationStatus == NotificationStatus.Pending),
                TotalSMSsSent = g.Count(n => n.PhoneNotificationStatus == NotificationStatus.Sent),
                TotalSMSsFailed = g.Count(n => n.PhoneNotificationStatus == NotificationStatus.Failed)
            }).FirstOrDefaultAsync(cancellationToken);

    public async Task AddAsync(Notification notification, CancellationToken cancellationToken = default)
    {
        await _context.AddAsync(notification, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task UpdateAsync(Notification notification, CancellationToken cancellationToken = default)
    {
        _context.Update(notification);
        await _context.SaveChangesAsync(cancellationToken);
    }
}
