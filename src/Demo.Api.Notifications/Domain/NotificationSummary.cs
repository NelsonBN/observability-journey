using System;

namespace Api.Notifications.Domain;

public sealed class NotificationSummary
{
    public required Guid Id { get; set; }
    public required Guid UserId { get; set; }
    public required NotificationStatus EmailNotificationStatus { get; set; }
    public required NotificationStatus PhoneNotificationStatus { get; set; }

    public required DateTime CreatedAt { get; set; }
}
