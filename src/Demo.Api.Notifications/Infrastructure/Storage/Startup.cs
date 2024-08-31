using System.Net;
using Azure.Storage.Blobs;

namespace Api.Notifications.Infrastructure.Storage;

internal sealed class Startup(
    BlobContainerClient client,
    ILogger<Startup> logger) : IStartup
{
    private readonly BlobContainerClient _client = client;
    private readonly ILogger<Startup> _logger = logger;

    public async Task ExecuteAsync(CancellationToken cancellationToken)
    {
        var containerExists = await _client.ExistsAsync(cancellationToken);
        if(containerExists.Value)
        {
            _logger.LogInformation("[INFRASTRUCTURE][STORAGE][STARTUP] The blob container already exists.");
            return;
        }

        // Create the container if it does not exist
        var createResponse = await _client.CreateIfNotExistsAsync(cancellationToken: cancellationToken);
        if(!((int)HttpStatusCode.Created).Equals(createResponse?.GetRawResponse()?.Status))
        {
            throw new InvalidOperationException("The blob container was not created.");
        }

        _logger.LogInformation("[INFRASTRUCTURE][STORAGE][STARTUP] The blob container '{ContainerName}' was created.", _client.Name);
    }
}
