using Api.Notifications.Domain;
using Microsoft.EntityFrameworkCore;

namespace Api.Notifications.Infrastructure.Database;

public sealed class NotificationsRepository(DataContext context) : INotificationsRepository
{
    private readonly DataContext _context = context;

    public Task<List<Notification>> ListAsync(CancellationToken cancellationToken = default)
        => _context
            .Notifications
            .AsNoTracking()
            .ToListAsync(cancellationToken);

    public Task<Notification?> GetAsync(Guid id, CancellationToken cancellationToken = default)
        => _context.Notifications.SingleOrDefaultAsync(s => s.Id == id, cancellationToken);

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
