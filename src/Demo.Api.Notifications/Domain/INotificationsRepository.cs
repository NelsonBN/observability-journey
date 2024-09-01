using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Api.Notifications.Domain;

public interface INotificationsRepository
{
    Task<List<Notification>> ListAsync(CancellationToken cancellationToken = default);
    Task<Notification?> GetAsync(Guid id, CancellationToken cancellationToken = default);
    Task AddAsync(Notification notification, CancellationToken cancellationToken = default);
    Task UpdateAsync(Notification notification, CancellationToken cancellationToken = default);
}
