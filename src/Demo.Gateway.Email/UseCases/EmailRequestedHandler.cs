using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using BuildingBlocks.Contracts;
using BuildingBlocks.Contracts.Abstractions;
using Gateway.Email.Domain;
using Microsoft.Extensions.Logging;

namespace Gateway.Email.UseCases;

public sealed class EmailRequestedHandler(
    ILogger<EmailRequestedHandler> logger,
    IStorageService storage) : IMessageHandler
{
    private readonly ILogger<EmailRequestedHandler> _logger = logger;
    private readonly IStorageService _storage = storage;

    public async Task HandleAsync(Message message, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("[HANDLER][EMAIL] received");

        var attachment = Convert.ToString(message.Data["Attachment"]);
        var email = Convert.ToString(message.Data["Email"]);
        var body = Convert.ToString(message.Data["Body"]);

        if(string.IsNullOrWhiteSpace(attachment))
        {
            _logger.LogInformation("[HANDLER][EMAIL][{Email}] {Body}", email, body);
        }
        else
        {
            var file = await _storage.GetAsync(attachment, cancellationToken);

            using var reader = new StreamReader(file);
            var content = await reader.ReadToEndAsync(cancellationToken);

            _logger.LogInformation("[HANDLER][EMAIL][{Email}] {Body} | Attachment: {Content}", email, body, content);
        }
    }
}
