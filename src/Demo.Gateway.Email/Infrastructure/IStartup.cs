namespace Gateway.Email.Infrastructure;

public interface IStartup
{
    Task ExecuteAsync(CancellationToken cancellationToken);
}
