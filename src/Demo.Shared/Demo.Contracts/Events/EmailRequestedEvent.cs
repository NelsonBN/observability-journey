using BuildingBlocks.Contracts.Abstractions;

namespace BuildingBlocks.Contracts.Events;

public sealed record EmailRequestedEvent : IMessage
{
    public required string? Email { get; init; }
    public required string Message { get; init; }
    public string? Attachment { get; init; }
}
