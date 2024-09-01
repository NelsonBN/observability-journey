using System;

namespace BuildingBlocks.Contracts.Abstractions;

public interface IMessage
{
    Guid Id { get; }
}
