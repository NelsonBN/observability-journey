using System;
using BuildingBlocks.Contracts.Abstractions;

namespace BuildingBlocks.Contracts.Events;

public sealed record EmailFeedbackEvent : IMessage
{
    public required Guid Id { get; init; }
    public required bool Success { get; init; }
}
