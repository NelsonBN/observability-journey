using Api.Notifications.Domain;
using BuildingBlocks.Exceptions;
using MediatR;
using Notifications;

namespace Api.Notifications.UseCases;

public sealed record GetNotificationsTotalsQuery(Guid UserId) : IRequest<NotificationsTotalsResponse>
{
    internal sealed class Handler(INotificationsRepository repository) : IRequestHandler<GetNotificationsTotalsQuery, NotificationsTotalsResponse>
    {
        private readonly INotificationsRepository _repository = repository;

        public async Task<NotificationsTotalsResponse> Handle(GetNotificationsTotalsQuery query, CancellationToken cancellationToken)
        {
            ExceptionFactory.ProbablyThrow<Handler>(35);

            var total = await _repository.GetTotalsAsync(query.UserId, cancellationToken);
            if(total is null)
            {
                return new()
                {
                    Emails = new()
                    {
                        TotalNoSent = 0,
                        TotalPending = 0,
                        TotalSent = 0,
                        TotalFailed = 0
                    },
                    SMSs = new()
                    {
                        TotalNoSent = 0,
                        TotalPending = 0,
                        TotalSent = 0,
                        TotalFailed = 0
                    }
                };
            }

            return new()
            {
                Emails = new()
                {
                    TotalNoSent = total.TotalEmailsNoSent,
                    TotalPending = total.TotalEmailsPending,
                    TotalSent = total.TotalEmailsSent,
                    TotalFailed = total.TotalEmailsFailed
                },
                SMSs = new()
                {
                    TotalNoSent = total.TotalSMSsNoSent,
                    TotalPending = total.TotalSMSsPending,
                    TotalSent = total.TotalSMSsSent,
                    TotalFailed = total.TotalSMSsFailed
                }
            };
        }
    }
}
