using Api.Users.Domain;
using Api.Users.DTOs;
using Common.Exceptions;
using MediatR;

namespace Api.Users.UseCases;

public sealed record GetUserNotificationsTotals(Guid Id) : IRequest<UserNotificationsTotalsResponse>
{
    internal sealed class Handler(IUsersRepository repository, INotificationsService service) : IRequestHandler<GetUserNotificationsTotals, UserNotificationsTotalsResponse>
    {
        private readonly IUsersRepository _repository = repository;
        private readonly INotificationsService _service = service;

        public async Task<UserNotificationsTotalsResponse> Handle(GetUserNotificationsTotals query, CancellationToken cancellationToken)
        {
            var user = await _repository.GetAsync(query.Id, cancellationToken);
            if(user is null)
            {
                throw new UserNotFoundException(query.Id);
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
}
