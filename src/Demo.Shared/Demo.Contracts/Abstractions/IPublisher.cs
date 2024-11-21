using System.Collections.Generic;
using System.Threading.Tasks;

namespace BuildingBlocks.Contracts.Abstractions;

public interface IPublisher
{
    Task Publish(params IEnumerable<Message> messages);

    Task Publish(Message message);
}
