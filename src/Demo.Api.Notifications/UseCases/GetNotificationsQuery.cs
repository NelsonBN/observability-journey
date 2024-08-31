using Api.Notifications.Domain;
using Api.Notifications.DTOs;
using BuildingBlocks.Exceptions;
using MediatR;

namespace Api.Notifications.UseCases;

public sealed record GetNotificationsQuery : IRequest<IEnumerable<NotificationResponse>>
{
    public static GetNotificationsQuery Instance => new();

    internal sealed class Handler(INotificationsRepository repository) : IRequestHandler<GetNotificationsQuery, IEnumerable<NotificationResponse>>
    {
        private readonly INotificationsRepository _repository = repository;

        public async Task<IEnumerable<NotificationResponse>> Handle(GetNotificationsQuery query, CancellationToken cancellationToken)
        {
            ExceptionFactory.ProbablyThrow<Handler>(35);

            var notifications = await _repository.ListAsync(cancellationToken);

            var result = notifications
                .Select(n => (NotificationResponse)n);

            return result;
        }
    }
}
