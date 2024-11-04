using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Api.Notifications.Domain;
using Api.Notifications.DTOs;
using BuildingBlocks.Contracts.Exceptions;

namespace Api.Notifications.UseCases;

public sealed record GetNotificationsQuery(INotificationsRepository Repository)
{
    private readonly INotificationsRepository _repository = Repository;

    public async Task<IEnumerable<NotificationResponse>> HandleAsync(CancellationToken cancellationToken)
    {
        ExceptionFactory.ProbablyThrow<GetNotificationsQuery>();

        var notifications = await _repository.ListAsync(cancellationToken);

        var result = notifications
            .Select(n => (NotificationResponse)n);

        return result;
    }
}
