using System;

namespace Api.Notifications.Domain;

public sealed record NotificationsTotals
{
    public Guid UserId { get; set; }

    public int TotalEmailsNoSent { get; set; }
    public int TotalEmailsPending { get; set; }
    public int TotalEmailsSent { get; set; }
    public int TotalEmailsFailed { get; set; }

    public int TotalSMSsNoSent { get; set; }
    public int TotalSMSsPending { get; set; }
    public int TotalSMSsSent { get; set; }
    public int TotalSMSsFailed { get; set; }
}
