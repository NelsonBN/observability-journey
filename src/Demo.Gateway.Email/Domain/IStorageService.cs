using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace Gateway.Email.Domain;

public interface IStorageService
{
    Task<Stream> GetAsync(string fileName, CancellationToken cancellationToken = default);
}
