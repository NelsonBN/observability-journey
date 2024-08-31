using System.Diagnostics;
using System.Net;
using System.Net.Mime;
using System.Text.Json;
using Api.Notifications.Domain;
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using BuildingBlocks.Observability;
using OpenTelemetry;
using OpenTelemetry.Context.Propagation;

namespace Api.Notifications.Infrastructure.Storage;

internal sealed class StorageService(BlobContainerClient client) : IStorageService
{
    private readonly BlobContainerClient _client = client;

    public async Task SaveAsync(Stream fileContent, string fileName, CancellationToken cancellationToken = default)
    {
        using var activity = Telemetry.Source.StartActivity($"BlobContainer {_client.Name}", ActivityKind.Producer);

        ActivityContext contextToInject = default;
        if(activity is not null)
        {
            contextToInject = activity.Context;
        }
        else if(Activity.Current is not null)
        {
            contextToInject = Activity.Current.Context;
        }

        var metadata = new Dictionary<string, string>();

        Propagators.DefaultTextMapPropagator.Inject(
            new(contextToInject, Baggage.Current),
            metadata,
            (mdata, key, value) => mdata[key] = value);


        var blobClient = _client.GetBlobClient(fileName);
        var response = (await blobClient.UploadAsync(
            fileContent,
            new BlobUploadOptions
            {
                HttpHeaders = new()
                {
                    ContentType = MediaTypeNames.Application.Json
                },
                Metadata = metadata
            },
            cancellationToken))
            .GetRawResponse();


        activity?
            .SetTag(TelemetrySemanticConventions.AzureStorage.SYSTEM, "azure-storage")
            .SetTag(TelemetrySemanticConventions.AzureStorage.OPERATION_TYPE, "publish")
            .SetTag(TelemetrySemanticConventions.AzureStorage.CONTAINER, _client.Name)
            .SetTag(TelemetrySemanticConventions.AzureStorage.BLOB, fileName)
            .AddAt();


        if(response?.Status != (int)HttpStatusCode.Created)
        {
            throw new InvalidOperationException(
                $"The file was not uploaded. Status: '{response?.Status}', Reason: '{response?.ReasonPhrase}'",
                new Exception(JsonSerializer.Serialize(response)));
        }
    }
}
