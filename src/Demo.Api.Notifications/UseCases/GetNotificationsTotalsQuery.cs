using System;
using System.Threading;
using System.Threading.Tasks;
using Api.Notifications.Domain;
using BuildingBlocks.Contracts.Exceptions;
using BuildingBlocks.Contracts.Notifications;

namespace Api.Notifications.UseCases;

public sealed record GetNotificationsTotalsQuery(IReportsRepository Repository)
{
    private readonly IReportsRepository _repository = Repository;

    public async Task<NotificationsTotalsResponse> HandleAsync(Guid userId, CancellationToken cancellationToken)
    {
        ExceptionFactory.ProbablyThrow<GetNotificationsTotalsQuery>(35);

        var total = await _repository.GetTotalsAsync(userId, cancellationToken);
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
