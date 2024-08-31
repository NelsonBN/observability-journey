using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;

namespace BuildingBlocks.MessageBus;

public sealed class MessageBusOptions
{
    public required string ExchangeName { get; init; }
    public required string QueueName { get; init; }

    public sealed class Setup(IConfiguration Configuration) : IConfigureOptions<MessageBusOptions>
    {
        public const string SECTION_NAME = "MessageBus";

        private readonly IConfiguration _configuration = Configuration;

        public void Configure(MessageBusOptions options)
            => _configuration.GetSection(SECTION_NAME)
                             .Bind(options);
    }
}
