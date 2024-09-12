using System.Threading;
using System.Threading.Tasks;

namespace BuildingBlocks.Contracts.Abstractions;

public interface IStartupService
{
    Task ExecuteAsync(CancellationToken cancellationToken);
}
