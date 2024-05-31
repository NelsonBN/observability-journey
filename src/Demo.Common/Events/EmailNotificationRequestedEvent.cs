namespace Common.Events;

public sealed record EmailNotificationRequestedEvent : DomainEvent
{
    public required string Message { get; init; }
    public required string? Email { get; init; }
}
