using System;
using System.Threading;
using System.Threading.Tasks;
using Api.Users.Domain;
using Api.Users.DTOs;
using BuildingBlocks.Contracts.Exceptions;

namespace Api.Users.UseCases;

public sealed class GetUserNotificationsTotalsQuery(
    IUsersRepository repository,
    INotificationsService service)
{
    private readonly IUsersRepository _repository = repository;
    private readonly INotificationsService _service = service;

    public async Task<UserNotificationsTotalsResponse> HandleAsync(Guid id, CancellationToken cancellationToken)
    {
        ExceptionFactory.ProbablyThrow<GetUserNotificationsTotalsQuery>();

        var user = await _repository.GetAsync(id, cancellationToken);
        if(user is null)
        {
            throw new UserNotFoundException(id);
        }

        var notificationsTotals = await _service.GetNotificationsTotalsAsync(user.Id, cancellationToken);

        return new(
            user.Id,
            user.Name,

            notificationsTotals.Emails.TotalNoSent,
            notificationsTotals.Emails.TotalPending,
            notificationsTotals.Emails.TotalSent,
            notificationsTotals.Emails.TotalFailed,

            notificationsTotals.SMSs.TotalNoSent,
            notificationsTotals.SMSs.TotalPending,
            notificationsTotals.SMSs.TotalSent,
            notificationsTotals.SMSs.TotalFailed);
    }
}
