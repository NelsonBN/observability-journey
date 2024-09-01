using System;
using BuildingBlocks.Contracts.Abstractions;

namespace BuildingBlocks.Contracts.Events;

public sealed record SMSNotificationRequestedEvent : IMessage
{
    public required Guid Id { get; init; }
    public required string Message { get; init; }
    public required string? Phone { get; init; }
}
