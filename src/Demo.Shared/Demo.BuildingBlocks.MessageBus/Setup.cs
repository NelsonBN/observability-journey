using System;
using BuildingBlocks.Contracts.Abstractions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Options;
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
                sp.GetRequiredService<IConnectionFactory>().CreateConnection())
            .AddTransient(sp =>
                sp.GetRequiredService<IConnection>().CreateModel())
            .AddTransient<IPublisher, Publisher>()
            .AddOptions<MessageBusOptions>().BindConfiguration(MessageBusOptions.SECTION_NAME);

        return services;
    }

    public static IServiceCollection AddConsumer<TMessage, THandler>(this IServiceCollection services, Func<IServiceProvider, MessageBusOptions>? factoryOptions = null)
        where TMessage : IMessage
        where THandler : class, IMessageHandler<TMessage>
        => services
            .AddMessageBus()
            .AddHostedService(sp =>
            {
                var options = factoryOptions is null ?
                    sp.GetRequiredService<IOptions<MessageBusOptions>>().Value :
                    factoryOptions(sp);

                return new Consumer<TMessage, THandler>(
                    sp,
                    options.ExchangeName,
                    options.QueueName);
            })
            .AddTransient<THandler>();



    public static IHealthChecksBuilder AddMessageBus(this IHealthChecksBuilder builder)
        => builder.AddRabbitMQ(
            "RabbitMQ",
            HealthStatus.Unhealthy);
}
