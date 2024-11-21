using BuildingBlocks.MessageBus;
using Gateway.Email.UseCases;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Gateway.Email.Infrastructure.MessageBus;

public static class Setup
{
    public static IServiceCollection AddMessageBus(this IServiceCollection services)
       => services
            .AddConsumer<EmailNotificationHandler>(sp => new()
            {
                ExchangeName = sp.GetRequiredService<IConfiguration>()[$"{MessageBusOptions.SECTION_NAME}:ExchangeName"]!,
                QueueName = "email-requested-queue",
                RoutingKey = "email.requested"
            })
            .AddConsumer<EmailRequestedHandler>(sp => new()
            {
                ExchangeName = sp.GetRequiredService<IConfiguration>()[$"{MessageBusOptions.SECTION_NAME}:ExchangeName"]!,
                QueueName = "report-queue",
                RoutingKey = "email.report"
            });
}
