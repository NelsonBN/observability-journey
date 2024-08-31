using MediatR;

namespace BuildingBlocks.Events;

public abstract record DomainEvent : INotification
{
    public required Guid Id { get; init; }
}
