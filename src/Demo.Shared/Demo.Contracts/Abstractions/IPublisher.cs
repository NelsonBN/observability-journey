using System.Collections.Generic;
using System.Threading.Tasks;

namespace BuildingBlocks.Contracts.Abstractions;

public interface IPublisher
{
    Task Publish<TMessage>(params IEnumerable<TMessage> messages)
        where TMessage : IMessage;

    Task Publish<TMessage>(TMessage message)
        where TMessage : IMessage;
}
