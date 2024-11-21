using System;
using BuildingBlocks.Contracts.Abstractions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using RabbitMQ.Client;

namespace BuildingBlocks.MessageBus;

public static class Setup
{
    public static IServiceCollection AddMessageBus(this IServiceCollection services)
    {
        services
            .AddSingleton<IConnectionFactory>(sp =>
                sp.GetRequiredService<IConfiguration>().GetSection(MessageBusOptions.SECTION_NAME).Get<ConnectionFactory>()!)
            .AddSingleton(sp =>
                sp.GetRequiredService<IConnectionFactory>().CreateConnectionAsync().GetAwaiter().GetResult())
            .AddTransient(sp =>
                sp.GetRequiredService<IConnection>().CreateChannelAsync().GetAwaiter().GetResult())
            .AddTransient<IPublisher, Publisher>()
            .AddOptions<MessageBusOptions>().BindConfiguration(MessageBusOptions.SECTION_NAME);

        return services;
    }

    public static IServiceCollection AddConsumer<THandler>(this IServiceCollection services, Func<IServiceProvider, MessageBusOptions> factoryOptions)
        where THandler : class, IMessageHandler
        => services
            .AddMessageBus()
            .AddHostedService(sp
                => new Consumer<THandler>(
                    sp,
                    factoryOptions(sp)))
            .AddTransient<THandler>();

    public static IHealthChecksBuilder AddMessageBus(this IHealthChecksBuilder builder)
        // TODO: Temporary disabled. Waiting for the implementation of the health check to compatible with new version of RabbitMQ Client
        => builder;
    //=> builder.AddRabbitMQ(
    //    "RabbitMQ",
    //    HealthStatus.Unhealthy);
}
