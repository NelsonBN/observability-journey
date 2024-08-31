namespace Api.Notifications.Domain;

public interface IReportsRepository
{
    Task<NotificationsTotals?> GetTotalsAsync(Guid userId, CancellationToken cancellationToken = default);
    Task<List<NotificationSummary>> NotificationSummariesAsync(DateTime startDateTime, DateTime endDateTime, CancellationToken cancellationToken = default);
    Task<ReportState?> GetStateAsync(CancellationToken cancellationToken = default);
    Task AddAsync(ReportState reportState, CancellationToken cancellationToken = default);
    Task UpdateAsync(ReportState reportState, CancellationToken cancellationToken = default);
}
