namespace BuildingBlocks.MessageBus;

public record MessageBusOptions
{
    public const string SECTION_NAME = "MessageBus";

    public required string ExchangeName { get; init; }
    public required string QueueName { get; init; }
    public required string RoutingKey { get; init; }
}
