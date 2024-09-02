using System.IO;
using System.Threading;
using System.Threading.Tasks;
using BuildingBlocks.Contracts.Abstractions;
using BuildingBlocks.Contracts.Events;
using Gateway.Email.Domain;
using Microsoft.Extensions.Logging;

namespace Gateway.Email.UseCases;

public class EmailRequestedHandler(
    ILogger<EmailRequestedHandler> logger,
    IStorageService storage)
    : IMessageHandler<EmailRequestedEvent>
{
    private readonly ILogger<EmailRequestedHandler> _logger = logger;
    private readonly IStorageService _storage = storage;

    public async Task HandleAsync(EmailRequestedEvent message, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("[EMAIL][HANDLER] received");


        if(string.IsNullOrWhiteSpace(message.Attachment))
        {
            _logger.LogError("[EMAIL][HANDLER] Attachment is empty");
        }
        else
        {
            var file = await _storage.GetAsync(message.Attachment, cancellationToken);

            using var reader = new StreamReader(file);
            var content = await reader.ReadToEndAsync(cancellationToken);

            _logger.LogInformation("[EMAIL][HANDLER] Attachment content: {Content}", content);
        }

        _logger.LogInformation("[EMAIL][HANDLER] handled");
    }
}
