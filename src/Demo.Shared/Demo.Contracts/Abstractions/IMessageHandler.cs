using System.Threading;
using System.Threading.Tasks;

namespace BuildingBlocks.Contracts.Abstractions;

public interface IMessageHandler
{
    public Task HandleAsync(Message message, CancellationToken cancellationToken = default);
}
