using BuildingBlocks.MessageBus;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using RabbitMQ.Client;

namespace Gateway.Email.Infrastructure.MessageBus;

public static class Setup
{
    public static IServiceCollection AddMessageBus(this IServiceCollection services)
        => services
            .ConfigureOptions<MessageBusOptions.Setup>()
            .AddSingleton<IConnectionFactory>(sp =>
                sp.GetRequiredService<IConfiguration>().GetSection(MessageBusOptions.Setup.SECTION_NAME).Get<ConnectionFactory>()!)
            .AddTransient(sp =>
                sp.GetRequiredService<IConnectionFactory>().CreateConnection())
            .AddTransient(sp =>
                sp.GetRequiredService<IConnection>().CreateModel())
            .AddTransient<IMessageBus, MessageBusServer>()
            .AddHostedService<ConsumerWorker>();

    public static IHealthChecksBuilder AddMessageBus(this IHealthChecksBuilder builder)
        => builder.AddRabbitMQ(
            "RabbitMQ",
            HealthStatus.Unhealthy);
}
