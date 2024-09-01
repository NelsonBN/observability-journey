using System;
using BuildingBlocks.Contracts.Abstractions;

namespace BuildingBlocks.Contracts.Events;

public sealed record SMSFeedbackEvent : IMessage
{
    public required Guid Id { get; init; }
    public required bool Success { get; init; }
}
