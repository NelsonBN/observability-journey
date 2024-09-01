using System.Threading;
using System.Threading.Tasks;

namespace BuildingBlocks.Contracts.Abstractions;

public interface IMessageHandler<TMessage>
    where TMessage : IMessage
{
    public Task HandleAsync(TMessage message, CancellationToken cancellationToken = default);
}
