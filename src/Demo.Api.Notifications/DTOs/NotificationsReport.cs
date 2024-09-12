using System;

namespace Api.Notifications.DTOs;

public sealed record class NotificationsReport
{
    public DateTime StartDateTime { get; init; }
    public DateTime EndDateTime { get; init; }

    public int TotalEmailsSent { get; set; }
    public int TotalEmailsFailed { get; set; }

    public int TotalSMSsSent { get; set; }
    public int TotalSMSsFailed { get; set; }

    public int TotalSent => TotalEmailsSent + TotalSMSsSent;
    public int TotalFailed => TotalEmailsFailed + TotalSMSsFailed;
}
