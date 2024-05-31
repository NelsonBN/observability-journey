namespace Api.Notifications.Domain;

public sealed class NotificationNotFoundException(Guid Id)
    : Exception($"Notification with id '{Id}' was not found")
{ }
