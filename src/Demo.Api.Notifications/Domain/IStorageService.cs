namespace Api.Notifications.Domain;

public interface IStorageService
{
    Task SaveAsync(Stream fileContent, string fileName, CancellationToken cancellationToken = default);
}
