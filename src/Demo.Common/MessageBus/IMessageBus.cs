using Common.Events;

namespace Common.MessageBus;

public interface IMessageBus
{
    void Publish<TMessage>(params TMessage[] domainEvents)
        where TMessage : DomainEvent;

    void Publish<TMessage>(TMessage domainEvent)
        where TMessage : DomainEvent;
}
