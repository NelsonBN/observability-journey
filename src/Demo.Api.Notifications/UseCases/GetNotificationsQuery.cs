using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Api.Notifications.Domain;
using Api.Notifications.DTOs;
using BuildingBlocks.Contracts.Exceptions;

namespace Api.Notifications.UseCases;

public sealed class GetNotificationsQuery(INotificationsRepository repository)
{
    private readonly INotificationsRepository _repository = repository;

    public async Task<IEnumerable<NotificationResponse>> HandleAsync(CancellationToken cancellationToken)
    {
        ExceptionFactory.ProbablyThrow<GetNotificationsQuery>();

        var notifications = await _repository.ListAsync(cancellationToken);

        var result = notifications
            .Select(n => (NotificationResponse)n);

        return result;
    }
}
