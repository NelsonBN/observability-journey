using System;
using System.Threading;
using System.Threading.Tasks;
using Api.Users.Domain;
using BuildingBlocks.Contracts.Notifications;

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
