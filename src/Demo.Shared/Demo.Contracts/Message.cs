using System;
using System.Collections.Generic;

namespace BuildingBlocks.Contracts;

public sealed record Message
{
    public Guid MessageId { get; init; } = Guid.NewGuid();
    public DateTime OccurredAt { get; init; } = DateTime.UtcNow;
    public required string Context { get; init; }
    public required string Type { get; init; }
    public string Version { get; init; } = "v1";

    public required Guid AggregateId { get; init; }

    public Dictionary<string, object> Data { get; init; } = [];
}
