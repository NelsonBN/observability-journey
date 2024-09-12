namespace BuildingBlocks.Contracts.Abstractions;

public interface IPublisher
{
    void Publish<TMessage>(params TMessage[] messages)
        where TMessage : IMessage;

    void Publish<TMessage>(TMessage message)
        where TMessage : IMessage;
}
