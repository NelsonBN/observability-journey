using System.Diagnostics;
using Azure.Storage.Blobs;
using BuildingBlocks.Observability;
using Gateway.Email.Domain;
using OpenTelemetry;
using OpenTelemetry.Context.Propagation;

namespace Gateway.Email.Infrastructure.Storage;

internal sealed class StorageService(BlobContainerClient client) : IStorageService
{
    private readonly BlobContainerClient _client = client;

    public async Task<Stream> GetAsync(string fileName, CancellationToken cancellationToken = default)
    {
        var blobClient = _client.GetBlobClient(fileName);
        if(!await blobClient.ExistsAsync(cancellationToken))
        {
            throw new FileNotFoundException("The file was not found.");
        }

        var properties = await blobClient.GetPropertiesAsync(cancellationToken: cancellationToken);
        var parentContext = Propagators.DefaultTextMapPropagator.Extract(
            default,
            properties.Value.Metadata,
            (mdata, key) =>
            {
                if(mdata.TryGetValue(key, out var value))
                {
                    return [value];
                }
                return [];
            });
        Baggage.Current = parentContext.Baggage;


        using var activity = Telemetry.Source.StartActivity($"BlobContainer {_client.Name}", ActivityKind.Consumer);


        var memoryStream = new MemoryStream();

        var response = await blobClient.DownloadToAsync(
            memoryStream,
            cancellationToken: cancellationToken);

        if(response.IsError)
        {
            throw new InvalidOperationException("The file was not downloaded.");
        }

        memoryStream.Position = 0; // Reset the position to the beginning of the stream

        activity?
            .SetTag(TelemetrySemanticConventions.AzureStorage.SYSTEM, "azure-storage")
            .SetTag(TelemetrySemanticConventions.AzureStorage.OPERATION_TYPE, "receive")
            .SetTag(TelemetrySemanticConventions.AzureStorage.CONTAINER, _client.Name)
            .SetTag(TelemetrySemanticConventions.AzureStorage.BLOB, fileName)
            .AddAt();

        return memoryStream;
    }
}
