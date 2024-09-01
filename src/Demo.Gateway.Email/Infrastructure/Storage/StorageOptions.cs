using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;

namespace Gateway.Email.Infrastructure.Storage;

internal sealed record StorageOptions
{
    public required string ContainerName { get; set; }

    internal sealed class Setup(IConfiguration configuration) : IConfigureOptions<StorageOptions>
    {
        internal const string SECTION_NAME = "Storage";

        private readonly IConfiguration _configuration = configuration;

        public void Configure(StorageOptions options)
            => _configuration.GetSection(SECTION_NAME)
                             .Bind(options);
    }
}
