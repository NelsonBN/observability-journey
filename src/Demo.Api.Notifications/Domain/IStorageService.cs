using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace Api.Notifications.Domain;

public interface IStorageService
{
    Task SaveAsync(Stream fileContent, string fileName, CancellationToken cancellationToken = default);
}
