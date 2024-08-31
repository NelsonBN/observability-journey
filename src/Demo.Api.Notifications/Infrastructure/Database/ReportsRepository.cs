using Api.Notifications.Domain;
using Microsoft.EntityFrameworkCore;

namespace Api.Notifications.Infrastructure.Database;

public sealed class ReportsRepository(DataContext context) : IReportsRepository
{
    private readonly DataContext _context = context;

    public async Task<NotificationsTotals?> GetTotalsAsync(Guid userId, CancellationToken cancellationToken = default)
        => await _context
            .Notifications
            .AsNoTracking()
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

    public Task<List<NotificationSummary>> NotificationSummariesAsync(DateTime startDateTime, DateTime endDateTime, CancellationToken cancellationToken = default)
        => _context
            .Notifications
            .AsNoTracking()
            .Where(n => n.CreatedAt > startDateTime && n.CreatedAt <= endDateTime)
            .Select(n => new NotificationSummary()
            {
                Id = n.Id,
                UserId = n.UserId,
                EmailNotificationStatus = n.EmailNotificationStatus,
                PhoneNotificationStatus = n.PhoneNotificationStatus,
                CreatedAt = n.CreatedAt
            })
            .ToListAsync(cancellationToken);

    public Task<ReportState?> GetStateAsync(CancellationToken cancellationToken = default)
        => _context.ReportState.SingleOrDefaultAsync(cancellationToken);

    public async Task AddAsync(ReportState reportState, CancellationToken cancellationToken = default)
    {
        await _context.AddAsync(reportState, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task UpdateAsync(ReportState reportState, CancellationToken cancellationToken = default)
    {
        _context.Update(reportState);
        await _context.SaveChangesAsync(cancellationToken);
    }
}
