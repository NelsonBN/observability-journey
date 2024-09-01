using BuildingBlocks.Contracts.Events;
using BuildingBlocks.MessageBus;
using Gateway.Email.UseCases;
using Microsoft.Extensions.DependencyInjection;

namespace Gateway.Email.Infrastructure.MessageBus;

public static class Setup
{
    public static IServiceCollection AddMessageBus(this IServiceCollection services)
       => services
            .AddConsumer<EmailNotificationRequestedEvent, EmailNotificationHandler>();
}
