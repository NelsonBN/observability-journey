using BuildingBlocks.MessageBus;
using Gateway.SMS.UseCases;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Gateway.SMS.Infrastructure.MessageBus;

public static class Setup
{
    public static IServiceCollection AddMessageBus(this IServiceCollection services)
       => services.AddConsumer<SMSNotificationHandler>(sp => new()
       {
           ExchangeName = sp.GetRequiredService<IConfiguration>()[$"{MessageBusOptions.SECTION_NAME}:ExchangeName"]!,
           QueueName = "sms-requested-queue",
           RoutingKey = "sms.requested"
       });
}
