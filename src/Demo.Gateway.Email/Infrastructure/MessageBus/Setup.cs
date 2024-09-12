using BuildingBlocks.Contracts.Events;
using BuildingBlocks.MessageBus;
using Gateway.Email.UseCases;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Gateway.Email.Infrastructure.MessageBus;

public static class Setup
{
    public static IServiceCollection AddMessageBus(this IServiceCollection services)
       => services
            .AddConsumer<EmailNotificationRequestedEvent, EmailNotificationHandler>(sp => new()
            {
                ExchangeName = sp.GetRequiredService<IConfiguration>()[$"{MessageBusOptions.SECTION_NAME}:ExchangeName"]!,
                QueueName = sp.GetRequiredService<IConfiguration>()[$"{MessageBusOptions.SECTION_NAME}:EmailNotificationsQueueName"]!
            })
            .AddConsumer<EmailRequestedEvent, EmailRequestedHandler>(sp => new()
            {
                ExchangeName = sp.GetRequiredService<IConfiguration>()[$"{MessageBusOptions.SECTION_NAME}:ExchangeName"]!,
                QueueName = sp.GetRequiredService<IConfiguration>()[$"{MessageBusOptions.SECTION_NAME}:EmailQueueName"]!
            });
}
