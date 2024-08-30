using MediatR;

namespace Common.Events;

public abstract record DomainEvent : INotification
{
    public required Guid Id { get; init; }
}
