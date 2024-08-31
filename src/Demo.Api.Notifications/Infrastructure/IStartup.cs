namespace Api.Notifications.Infrastructure;

public interface IStartup
{
    Task ExecuteAsync(CancellationToken cancellationToken);
}
