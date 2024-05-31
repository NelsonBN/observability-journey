using Api.Users.Domain;
using Notifications;

namespace Api.Users.Infrastructure.NotificationsApi;

internal sealed class NotificationsService(Greeter.GreeterClient client) : INotificationsService
{
    private readonly Greeter.GreeterClient _client = client;

    public async Task<NotificationsTotalsResponse> GetNotificationsTotalsAsync(Guid userId, CancellationToken cancellationToken)
        => await _client.GetNotificationsTotalsAsync(new()
        {
            UserId = userId.ToString()
        },
        cancellationToken: cancellationToken);
}
