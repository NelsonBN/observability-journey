using System;
using Api.Notifications.Domain;

namespace Api.Notifications.DTOs;

public sealed record NotificationResponse(
    Guid Id,
    Guid UserId,
    string Body,
    string? Email,
    NotificationStatus EmailNotificationStatus,
    string? Phone,
    NotificationStatus PhoneNotificationStatus)
{
    public static implicit operator NotificationResponse(Notification notification)
        => new(
            notification.Id,
            notification.UserId,
            notification.Body,
            notification.Email,
            notification.EmailNotificationStatus,
            notification.Phone,
            notification.PhoneNotificationStatus);
}
