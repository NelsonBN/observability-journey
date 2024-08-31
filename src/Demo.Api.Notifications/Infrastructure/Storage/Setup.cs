using Api.Notifications.Domain;
using Azure.Storage.Blobs;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Options;

namespace Api.Notifications.Infrastructure.Storage;

internal static class Setup
{
    internal static IServiceCollection AddStorage(this IServiceCollection services)
        => services
            .ConfigureOptions<StorageOptions.Setup>()
            .AddTransient<IStartup, Startup>()
            .AddTransient<IStorageService, StorageService>()
            .AddSingleton(sp =>
                new BlobContainerClient(
                    sp.GetRequiredService<IConfiguration>().GetConnectionString(StorageOptions.Setup.SECTION_NAME),
                    sp.GetRequiredService<IOptions<StorageOptions>>().Value.ContainerName,
                    new()
                    {
                        Retry =
                        {
                            MaxRetries = 3,
                            Delay = TimeSpan.FromSeconds(3),
                            MaxDelay = TimeSpan.FromSeconds(10),
                            Mode = Azure.Core.RetryMode.Exponential,
                            NetworkTimeout = TimeSpan.FromSeconds(30)
                        }
                    }))
            .AddSingleton(sp =>
                new BlobServiceClient(sp.GetRequiredService<IConfiguration>()
                    .GetConnectionString(StorageOptions.Setup.SECTION_NAME)));

    public static IHealthChecksBuilder AddStorage(this IHealthChecksBuilder builder)
        => builder.AddAzureBlobStorage(
            name: "Storage",
            clientFactory: (ser) => ser.CreateScope().ServiceProvider.GetRequiredService<BlobServiceClient>(),
            failureStatus: HealthStatus.Unhealthy);
}
