using Api.Notifications.UseCases;
using BuildingBlocks.MessageBus;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Api.Notifications.Infrastructure.MessageBus;

public static class Setup
{
    public static IServiceCollection AddMessageBus(this IServiceCollection services)
        => services
            .AddConsumer<EmailResponseHandler>(sp => new()
            {
                ExchangeName = sp.GetRequiredService<IConfiguration>()[$"{MessageBusOptions.SECTION_NAME}:ExchangeName"]!,
                QueueName = "email-response-queue",
                RoutingKey = "email.response"
            })
            .AddConsumer<SMSResponseHandler>(sp => new()
            {
                ExchangeName = sp.GetRequiredService<IConfiguration>()[$"{MessageBusOptions.SECTION_NAME}:ExchangeName"]!,
                QueueName = "sms-response-queue",
                RoutingKey = "sms.response"
            });
}
