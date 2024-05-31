using Api.Notifications.Domain;
using Api.Notifications.DTOs;
using MediatR;

namespace Api.Notifications.UseCases;

public sealed record GetNotificationQuery(Guid Id) : IRequest<NotificationResponse>
{
    internal sealed class Handler(INotificationsRepository repository) : IRequestHandler<GetNotificationQuery, NotificationResponse>
    {
        private readonly INotificationsRepository _repository = repository;

        public async Task<NotificationResponse> Handle(GetNotificationQuery query, CancellationToken cancellationToken)
        {
            var notification = await _repository.GetAsync(query.Id, cancellationToken);
            if(notification is null)
            {
                throw new NotificationNotFoundException(query.Id);
            }

            return notification;
        }
    }
}
