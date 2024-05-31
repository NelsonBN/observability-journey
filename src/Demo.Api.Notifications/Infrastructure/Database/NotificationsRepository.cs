using Api.Notifications.Domain;
using Api.Notifications.Infrastructure.Cache;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Distributed;

namespace Api.Notifications.Infrastructure.Database;

public sealed class NotificationsRepository(
    DataContext context,
    IDistributedCache cache) : INotificationsRepository
{
    private static readonly DistributedCacheEntryOptions _cacheOptions = new()
    {
        AbsoluteExpirationRelativeToNow = TimeSpan.FromSeconds(30),
        SlidingExpiration = TimeSpan.FromSeconds(15)
    };


    private readonly DataContext _context = context;
    private readonly IDistributedCache _cache = cache;

    public Task<List<Notification>> ListAsync(CancellationToken cancellationToken = default)
        => _context.Notifications.ToListAsync(cancellationToken);

    public Task<Notification?> GetAsync(Guid id, CancellationToken cancellationToken = default)
        => _context.Notifications.SingleOrDefaultAsync(s => s.Id == id, cancellationToken);

    public async Task<NotificationsTotals?> GetTotalsAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        var key = $"{nameof(userId)}:{userId}";
        var totals = await _cache.GetAsync<NotificationsTotals>(key, cancellationToken);


        if(totals is null)
        {
            totals = await _context.Notifications
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

            if(totals is not null)
            {
                await _cache.SetAsync(
                    key,
                    totals,
                    _cacheOptions,
                    cancellationToken);
            }
        }


        return totals;
    }

    public async Task AddAsync(Notification notification, CancellationToken cancellationToken = default)
    {
        await _invalidateCache(notification.UserId, cancellationToken);

        await _context.AddAsync(notification, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task UpdateAsync(Notification notification, CancellationToken cancellationToken = default)
    {
        await _invalidateCache(notification.UserId, cancellationToken);

        _context.Update(notification);
        await _context.SaveChangesAsync(cancellationToken);
    }


    private async Task _invalidateCache(Guid userId, CancellationToken cancellationToken)
    {
        var key = $"{nameof(userId)}:{userId}";
        await _cache.RemoveAsync(key, cancellationToken);
    }
}
