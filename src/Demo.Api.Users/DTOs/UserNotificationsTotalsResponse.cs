namespace Api.Users.DTOs;

public sealed record UserNotificationsTotalsResponse(
    Guid Id,
    string Name,

    int TotalEmailsNoSent,
    int TotalEmailsPending,
    int TotalEmailsSent,
    int TotalEmailsFailed,

    int TotalSMSsNoSent,
    int TotalSMSsPending,
    int TotalSMSsSent,
    int TotalSMSsFailed);
