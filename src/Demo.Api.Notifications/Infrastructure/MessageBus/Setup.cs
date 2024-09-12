using Api.Notifications.UseCases;
using BuildingBlocks.Contracts.Events;
using BuildingBlocks.MessageBus;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Api.Notifications.Infrastructure.MessageBus;

public static class Setup
{
    public static IServiceCollection AddMessageBus(this IServiceCollection services)
        => services
            .AddConsumer<EmailFeedbackEvent, EmailFeedbackHandler>(sp => new()
            {
                ExchangeName = sp.GetRequiredService<IConfiguration>()[$"{MessageBusOptions.SECTION_NAME}:ExchangeName"]!,
                QueueName = sp.GetRequiredService<IConfiguration>()[$"{MessageBusOptions.SECTION_NAME}:EmailFeedbackQueueName"]!
            })
            .AddConsumer<SMSFeedbackEvent, SMSFeedbackHandler>(sp => new()
            {
                ExchangeName = sp.GetRequiredService<IConfiguration>()[$"{MessageBusOptions.SECTION_NAME}:ExchangeName"]!,
                QueueName = sp.GetRequiredService<IConfiguration>()[$"{MessageBusOptions.SECTION_NAME}:SMSFeedbackQueueName"]!
            });
}
