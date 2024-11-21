using System;

namespace Api.Notifications.DTOs;

public sealed record NotificationRequest(Guid UserId, string Body);
