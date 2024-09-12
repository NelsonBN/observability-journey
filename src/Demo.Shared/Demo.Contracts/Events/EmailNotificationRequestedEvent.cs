using System;
using BuildingBlocks.Contracts.Abstractions;

namespace BuildingBlocks.Contracts.Events;

public sealed record EmailNotificationRequestedEvent : IMessage
{
    public required Guid Id { get; init; }
    public required string? Email { get; init; }
    public required string Message { get; init; }
}
