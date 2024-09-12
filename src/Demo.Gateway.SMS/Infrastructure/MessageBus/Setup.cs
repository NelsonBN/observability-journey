using BuildingBlocks.Contracts.Events;
using BuildingBlocks.MessageBus;
using Gateway.SMS.UseCases;
using Microsoft.Extensions.DependencyInjection;

namespace Gateway.SMS.Infrastructure.MessageBus;

public static class Setup
{
    public static IServiceCollection AddMessageBus(this IServiceCollection services)
       => services
            .AddConsumer<SMSNotificationRequestedEvent, SMSNotificationHandler>();
}
