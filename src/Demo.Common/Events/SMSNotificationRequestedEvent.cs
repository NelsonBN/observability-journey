namespace Common.Events;

public sealed record SMSNotificationRequestedEvent : DomainEvent
{
    public required string Message { get; init; }
    public required string? Phone { get; init; }
}
